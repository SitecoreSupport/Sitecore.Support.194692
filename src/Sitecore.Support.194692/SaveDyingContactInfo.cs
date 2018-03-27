using Sitecore.Analytics.Pipelines.MergeContacts;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
    public class SaveDyingContactInfo : MergeContactProcessor
    {
        // Fields
        private readonly DyingContactSessionManager _dyingContactStore;

        // Methods
        public SaveDyingContactInfo(DyingContactSessionManager dyingContactStore)
        {
            Assert.ArgumentNotNull(dyingContactStore, "dyingContactStore");
            this._dyingContactStore = dyingContactStore;
        }

        public override void Process(MergeContactArgs args)
        {
            if (args.DyingContact.ContactId != args.SurvivingContact.ContactId)
            {
                this._dyingContactStore.SetContactDyingInfo(args.DyingContact.ContactId, args.SurvivingContact.ContactId);
            }
        }
    }

}