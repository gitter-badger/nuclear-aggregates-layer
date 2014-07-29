using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
	public sealed class ContactIdIdentity : EntityPropertyIdentityBase<ContactIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.ContactIdPropertyId; }
        }

        public override string Description
        {
			get { return PropertyName; }
        }

        public override string PropertyName
        {
			get { return "ContactId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long?); }
        }
    }
}