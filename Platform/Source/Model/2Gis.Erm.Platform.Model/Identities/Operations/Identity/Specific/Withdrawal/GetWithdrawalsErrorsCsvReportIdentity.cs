using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class GetWithdrawalsErrorsCsvReportIdentity : OperationIdentityBase<GetWithdrawalsErrorsCsvReportIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetWithdrawalsErrorsCsvReportIdentity; }
        }

        public override string Description
        {
            get { return "GetWithdrawalsErrorsCsvReport"; }
        }
    }
}
