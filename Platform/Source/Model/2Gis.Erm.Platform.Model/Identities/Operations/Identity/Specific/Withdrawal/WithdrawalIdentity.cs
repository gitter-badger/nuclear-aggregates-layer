using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class WithdrawalIdentity : OperationIdentityBase<WithdrawalIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return NuClear.Model.Common.Operations.Identity.OperationIdentityIds.WithdrawalIdentity; }
        }

        public override string Description
        {
            get { return "Withdrawal"; }
        }
    }
}
