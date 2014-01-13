using System.ServiceModel;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [MessageContract(IsWrapped = true)]
    public sealed class MoneyDistributionReportResponse
    {
        [MessageBodyMember]
        public MoneyDistributionReportItem[] MoneyDistributionReportItems { get; set; }
    }
}