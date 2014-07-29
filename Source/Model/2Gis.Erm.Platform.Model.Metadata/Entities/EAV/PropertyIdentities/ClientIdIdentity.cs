using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
	public sealed class ClientIdIdentity : EntityPropertyIdentityBase<ClientIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ClientIdPropertyId; }
        }

        public override string Description
        {
			get { return PropertyName; }
        }

        public override string PropertyName
        {
			get { return "ClientId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long?); }
        }
    }
}