using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeDealsDuringWithdrawalIdentity : OperationIdentityBase<ActualizeDealsDuringWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeDealsDuringWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize deals state during withdrawal process"; }
        }
    }
}
