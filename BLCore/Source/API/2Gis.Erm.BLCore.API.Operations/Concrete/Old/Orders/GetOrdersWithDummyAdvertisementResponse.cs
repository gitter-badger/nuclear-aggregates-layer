using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class GetOrdersWithDummyAdvertisementResponse : Response
    {
        public byte[] ReportContent { get; set; }
        public string ReportFileName { get; set; }
        public string ContentType { get; set; }
        public bool HasOrders { get; set; }
        public string Message { get; set; }
    }
}
