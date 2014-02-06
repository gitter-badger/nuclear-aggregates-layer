using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class ScheduledEndIdentity : EntityPropertyIdentityBase<ScheduledEndIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ScheduledEndPropertyId; }
        }

        public override string Description
        {
            get { return "ScheduledEnd"; }
        }

        public override string PropertyName
        {
            get { return "ScheduledEnd"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}