using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class WithdrawalIdentity : OperationIdentityBase<WithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.WithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Withdrawal"; }
        }
    }
}
