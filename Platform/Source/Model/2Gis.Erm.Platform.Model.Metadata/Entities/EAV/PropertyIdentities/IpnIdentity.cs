using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class IpnIdentity : EntityPropertyIdentityBase<IpnIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IpnPropertyId; }
        }

        public override string Description
        {
            get { return "Ipn"; }
        }

        public override string PropertyName
        {
            get { return "Ipn"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}