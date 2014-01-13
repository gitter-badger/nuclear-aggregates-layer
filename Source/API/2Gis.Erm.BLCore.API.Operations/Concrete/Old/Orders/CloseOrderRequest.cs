using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class CloseOrderRequest : Request
    {
        public long OrderId { get; set; }
        public string Reason { get; set; }
    }
}
