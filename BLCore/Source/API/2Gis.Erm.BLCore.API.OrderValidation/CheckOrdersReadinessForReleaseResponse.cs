using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class CheckOrdersReadinessForReleaseResponse : Response
    {
        public byte[] ReportContent { get; set; }
        public string ReportFileName { get; set; }
        public string ContentType { get; set; }
        public bool HasErrors { get; set; }
        public string Message { get; set; }
    }
}