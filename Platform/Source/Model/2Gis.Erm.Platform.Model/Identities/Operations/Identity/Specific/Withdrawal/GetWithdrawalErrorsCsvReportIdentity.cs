using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class GetWithdrawalErrorsCsvReportIdentity : OperationIdentityBase<GetWithdrawalErrorsCsvReportIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetWithdrawalErrorsCsvReportIdentity; }
        }

        public override string Description
        {
            get { return "GetWithdrawalErrorsCsvReport"; }
        }
    }
}
