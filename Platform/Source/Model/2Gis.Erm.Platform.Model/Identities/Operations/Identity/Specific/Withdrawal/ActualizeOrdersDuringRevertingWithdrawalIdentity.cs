using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeOrdersDuringRevertingWithdrawalIdentity : OperationIdentityBase<ActualizeOrdersDuringRevertingWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeOrdersDuringRevertingWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize orders state during reverting withdrawal process. Recalculate amounts, actualize workflow statuses and etc"; }
        }
    }
}
