using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class RevertWithdrawalIdentity : OperationIdentityBase<RevertWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RevertWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "RevertWithdrawal"; }
        }
    }
}
