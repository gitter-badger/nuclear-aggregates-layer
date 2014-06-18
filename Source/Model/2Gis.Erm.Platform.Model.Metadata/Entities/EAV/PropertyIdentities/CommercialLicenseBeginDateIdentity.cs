using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class CommercialLicenseBeginDateIdentity : EntityPropertyIdentityBase<CommercialLicenseBeginDateIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.CommercialLicenseBeginDatePropertyId; }
        }

        public override string Description
        {
            get { return "Commercial license begin date "; }
        }

        public override string PropertyName
        {
            get { return "CommercialLicenseBeginDate"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime); }
        }
    }
}