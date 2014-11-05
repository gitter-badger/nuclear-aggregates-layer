using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public class IdentityCardIssuedOnIdentity : EntityPropertyIdentityBase<IdentityCardIssuedOnIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.IdentityCardIssuedOnPropertyId; }
        }

        public override string Description
        {
            get { return "Дата выдачи"; }
        }

        public override string PropertyName
        {
            get { return "IdentityCardIssuedOn"; }
        }

        public override Type PropertyType
        {
            get { return typeof(DateTime?); }
        }
    }
}