using System.ServiceModel;

namespace DoubleGis.Erm.BLCore.API.MoDi.Reports
{
    [MessageContract(IsWrapped = true)]
    public sealed class PlatformReportResponse
    {
        [MessageBodyMember]
        public PlatformReportItem[] PlatformReportItems { get; set; }
    }
}