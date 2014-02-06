using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class AfterSaleServiceTypeIdentity : EntityPropertyIdentityBase<AfterSaleServiceTypeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.AfterSaleServiceTypePropertyId; }
        }

        public override string Description
        {
            get { return "AfterSaleServiceType"; }
        }

        public override string PropertyName
        {
            get { return "AfterSaleServiceType"; }
        }

        public override Type PropertyType
        {
            get { return typeof(byte); }
        }
    }
}