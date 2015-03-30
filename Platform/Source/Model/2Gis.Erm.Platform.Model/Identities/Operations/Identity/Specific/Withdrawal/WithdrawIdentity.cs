using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class WithdrawIdentity : OperationIdentityBase<WithdrawIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.WithdrawIdentity; }
        }

        public override string Description
        {
            get { return "Withdrawal"; }
        }
    }
}
