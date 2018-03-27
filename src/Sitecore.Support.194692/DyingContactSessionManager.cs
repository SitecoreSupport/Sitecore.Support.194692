using System;
using System.Configuration.Provider;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;
using Sitecore.Analytics.Tracking.SharedSessionState;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Sitecore.Support
{
    public class DyingContactSessionManager
    {
        // Fields
        private readonly SharedSessionStateConfigBase _configuration;
        private readonly SessionStateStoreProviderBase _provider;
        private static readonly ProviderHelper<SessionStateStoreProviderBase, ProviderCollection> ProviderHelper = new ProviderHelper<SessionStateStoreProviderBase, ProviderCollection>("tracking/sharedSessionState");
        private const string SurvivedContactIdCacheKey = "0";

        // Methods
        public DyingContactSessionManager(SharedSessionStateConfigBase configuration) : this(ProviderHelper.Provider, configuration)
        {
        }

        public DyingContactSessionManager(SessionStateStoreProviderBase sharedSessionStateProvider, SharedSessionStateConfigBase configuration)
        {
            Assert.ArgumentNotNull(sharedSessionStateProvider, "sharedSessionStateProvider");
            Assert.ArgumentNotNull(configuration, "configuration");
            this._provider = sharedSessionStateProvider;
            this._configuration = configuration;
        }

        private static string GetCacheKey(Guid contactId) =>
            $"{contactId}_DyingInfo";

        private static HttpContext GetHttpContext()
        {
            SimpleWorkerRequest request;
            if (HttpContext.Current != null)
            {
                return HttpContext.Current;
            }
            try
            {
                request = new SimpleWorkerRequest("/", AppDomain.CurrentDomain.BaseDirectory, "/", string.Empty, TextWriter.Null);
            }
            catch (HttpException)
            {
                request = new SimpleWorkerRequest("/", string.Empty, TextWriter.Null);
            }
            return new HttpContext(request);
        }

        public void SetContactDyingInfo(Guid dyingContactId, Guid survivingContactId)
        {
            string cacheKey = GetCacheKey(dyingContactId);
            SessionStateStoreData item = this._provider.CreateNewStoreData(GetHttpContext(), this._configuration.Timeout);
            item.Items["0"] = survivingContactId;
            this._provider.SetAndReleaseItemExclusive(GetHttpContext(), cacheKey, item, null, true);
        }

        public bool TryGetSurvivedContactId(Guid dyingContactId, out Guid survivedContactId)
        {
            bool flag;
            TimeSpan span;
            object obj2;
            SessionStateActions actions;
            string cacheKey = GetCacheKey(dyingContactId);
            obj2 = this._provider.GetItem(GetHttpContext(), cacheKey, out flag, out span, out obj2, out actions)?.Items["0"];
            bool flag1 = obj2 is Guid;
            Guid guid = flag1 ? ((Guid)obj2) : new Guid();
            if (flag1)
            {
                survivedContactId = guid;
                return true;
            }
            survivedContactId = Guid.Empty;
            return false;
        }
    }
}