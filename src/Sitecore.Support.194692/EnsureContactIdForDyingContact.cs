using Sitecore.Analytics.Pipelines.InitializeTracker;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
    public class EnsureContactIdForDyingContact : InitializeTrackerProcessor
    {
        // Fields
        private readonly DyingContactSessionManager _dyingContactStore;

        // Methods
        public EnsureContactIdForDyingContact(DyingContactSessionManager dyingContactStore)
        {
            Assert.ArgumentNotNull(dyingContactStore, "dyingContactStore");
            this._dyingContactStore = dyingContactStore;
        }

        public override void Process(InitializeTrackerArgs args)
        {
            if (args.ContactId.HasValue)
            {
                Guid guid;
                Guid? contactId = args.ContactId;
                if (this._dyingContactStore.TryGetSurvivedContactId(contactId.Value, out guid))
                {
                    contactId = new Guid?(guid);
                    args.Session.Device.LastKnownContactId = contactId;
                    args.ContactId = contactId;
                }
            }
        }
    }
}
