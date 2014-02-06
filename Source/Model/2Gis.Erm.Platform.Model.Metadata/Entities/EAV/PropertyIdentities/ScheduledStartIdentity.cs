using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class ScheduledStartIdentity : EntityPropertyIdentityBase<ScheduledStartIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ScheduledStartPropertyId; }
        }

        public override string Description 
        {
            get { return "ScheduledStart"; }
        }

        public override string PropertyName
        {
            get { return "ScheduledStart"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}