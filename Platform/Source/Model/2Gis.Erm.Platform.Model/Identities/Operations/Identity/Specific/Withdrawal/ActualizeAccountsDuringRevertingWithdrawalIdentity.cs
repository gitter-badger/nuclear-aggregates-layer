using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeAccountsDuringRevertingWithdrawalIdentity : OperationIdentityBase<ActualizeAccountsDuringRevertingWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeAccountsDuringRevertingWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize accounts state during reverting withdrawal process, modify accounts, locks etc"; }
        }
    }
}
