using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class AuthorIdIdentity : EntityPropertyIdentityBase<AuthorIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.AuthorIdPropertyId; }
        }

        public override string Description
        {
            get { return "AuthorId"; }
        }

        public override string PropertyName
        {
            get { return "AuthorId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long); }
        }
    }
}