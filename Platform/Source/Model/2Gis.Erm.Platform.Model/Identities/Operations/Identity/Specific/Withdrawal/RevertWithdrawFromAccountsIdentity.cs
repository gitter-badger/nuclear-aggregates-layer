using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class RevertWithdrawFromAccountsIdentity : OperationIdentityBase<RevertWithdrawFromAccountsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RevertWithdrawFromAccountsIdentity; }
        }

        public override string Description
        {
            get { return "Revert withdrawal from accounts"; }
        }
    }
}
