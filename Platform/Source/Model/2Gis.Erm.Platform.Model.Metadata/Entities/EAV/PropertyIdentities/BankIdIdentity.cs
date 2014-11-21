using System;

using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class BankIdIdentity : EntityPropertyIdentityBase<BankIdIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.BankIdPropertyId; }
        }

        public override string Description
        {
            get { return "BankId"; }
        }

        public override string PropertyName
        {
            get { return "BankId"; }
        }

        public override Type PropertyType
        {
            get { return typeof(long?); }
        }
    }
}