using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class ActualAddressIdentity : EntityPropertyIdentityBase<ActualAddressIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ActualAddressPropertyId; }
        }

        public override string Description
        {
            get { return "Фактический адрес"; }
        }

        public override string PropertyName
        {
            get { return "ActualAddress"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}