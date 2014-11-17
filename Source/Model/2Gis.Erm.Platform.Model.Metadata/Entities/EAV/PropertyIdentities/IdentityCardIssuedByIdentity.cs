using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class IdentityCardIssuedByIdentity : EntityPropertyIdentityBase<IdentityCardIssuedByIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IdentityCardIssuedByPropertyId; }
        }

        public override string Description
        {
            get { return "Выдан"; }
        }

        public override string PropertyName
        {
            get { return "IdentityCardIssuedBy"; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}