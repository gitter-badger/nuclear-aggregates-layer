using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeOrdersDuringWithdrawalIdentity : OperationIdentityBase<ActualizeOrdersDuringWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeOrdersDuringWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize orders state during withdrawal process"; }
        }
    }
}
