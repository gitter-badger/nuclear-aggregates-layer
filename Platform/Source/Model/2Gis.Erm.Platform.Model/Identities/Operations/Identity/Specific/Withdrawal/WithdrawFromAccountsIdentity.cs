﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class WithdrawFromAccountsIdentity : OperationIdentityBase<WithdrawFromAccountsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.WithdrawFromAccountsIdentity; }
        }

        public override string Description
        {
            get { return "Make withdraw from accounts"; }
        }
    }
}
