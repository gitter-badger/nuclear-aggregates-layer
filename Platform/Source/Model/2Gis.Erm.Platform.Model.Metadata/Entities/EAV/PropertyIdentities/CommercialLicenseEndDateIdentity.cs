using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class CommercialLicenseEndDateIdentity : EntityPropertyIdentityBase<CommercialLicenseEndDateIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.CommercialLicenseEndDatePropertyId; }
        }

        public override string Description
        {
            get { return "Commercial license end date "; }
        }

        public override string PropertyName
        {
            get { return "CommercialLicenseEndDate"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}