using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class WithdrawalsByAccountingMethodIdentity : OperationIdentityBase<WithdrawalsByAccountingMethodIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.WithdrawalsByAccountingMethodIdentity; }
        }

        public override string Description
        {
            get { return "WithdrawalsByAccountingMethod"; }
        }
    }
}
