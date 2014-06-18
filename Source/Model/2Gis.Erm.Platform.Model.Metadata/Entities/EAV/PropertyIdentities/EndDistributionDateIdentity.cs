using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class EndDistributionDateIdentity : EntityPropertyIdentityBase<EndDistributionDateIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.EndDistributionDatePropertyId; }
        }

        public override string Description
        {
            get { return "EndDistributionDate"; }
        }

        public override string PropertyName
        {
            get { return "EndDistributionDate"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}