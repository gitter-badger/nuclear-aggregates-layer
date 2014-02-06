using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class PriorityIdentity : EntityPropertyIdentityBase<PriorityIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.PriorityPropertyId; }
        }

        public override string Description 
        {
            get { return "Priority"; }
        }

        public override string PropertyName
        {
            get { return "Priority"; }
        }

        public override Type PropertyType
        {
            get { return typeof(ActivityPriority); }
        }
    }
}