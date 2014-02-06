using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class AccountTypeIdentity : EntityPropertyIdentityBase<AccountTypeIdentity>
    {
        public override int Id
        {
            get { return PropertyIdentityIds.AccountTypePropertyId; }
        }

        public override string Description
        {
            get { return "AccountType"; }
        }

        public override string PropertyName
        {
            get { return "AccountType"; }
        }

        public override Type PropertyType
        {
            get { return typeof(AccountType); }
        }
    }
}