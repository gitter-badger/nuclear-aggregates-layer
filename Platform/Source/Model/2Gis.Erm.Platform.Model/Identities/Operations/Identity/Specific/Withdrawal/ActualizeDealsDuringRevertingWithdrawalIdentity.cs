using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class ActualizeDealsDuringRevertingWithdrawalIdentity : OperationIdentityBase<ActualizeDealsDuringRevertingWithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ActualizeDealsDuringRevertingWithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Actualize deals state during reverting withdrawal process"; }
        }
    }
}
