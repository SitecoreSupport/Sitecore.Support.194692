using Sitecore.Analytics.Pipelines.InitializeTracker;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.EnsureSessionContext.EnsureContactId
{
    public class EnsureContactId : InitializeTrackerProcessor
    {
        // Methods
        public override void Process(InitializeTrackerArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Session, "args.Session");
            Assert.IsNotNull(args.Session.Device, "args.Session.Device");
            if (args.Session.Contact != null)
            {
                args.ContactId = new Guid?(args.Session.Contact.ContactId);
            }
            if (args.ContactId.HasValue && (args.ContactId.Value != Guid.Empty))
            {
                args.Session.Device.LastKnownContactId = new Guid?(args.ContactId.Value);
            }
            else if (args.Session.Device.LastKnownContactId.HasValue && (args.Session.Device.LastKnownContactId.Value != Guid.Empty))
            {
                args.ContactId = args.Session.Device.LastKnownContactId;
            }
            else
            {
                args.ContactId = args.Session.Device.LastKnownContactId = new Guid?(Guid.NewGuid());
                args.IsNewContact = true;
            }
        }
    }

}