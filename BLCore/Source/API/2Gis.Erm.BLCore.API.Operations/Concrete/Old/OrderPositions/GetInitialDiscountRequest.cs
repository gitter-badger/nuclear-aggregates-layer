using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class GetInitialDiscountRequest : Request
    {
        public long OrderId { get; set; }
    }
}
