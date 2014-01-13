using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    public sealed class RemoveBargainFromOrderRequest : Request
    {
        public long OrderId { get; set; }
    }
}
