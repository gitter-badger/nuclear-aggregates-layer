using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class TaxationTypeIdentity : EntityPropertyIdentityBase<TaxationTypeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.TaxationTypePropertyId; }
        }

        public override string Description
        {
            get { return "TaxationType"; }
        }

        public override string PropertyName
        {
            get { return "TaxationType"; }
        }

        public override Type PropertyType
        {
            get { return typeof(TaxationType); }
        }
    }
}