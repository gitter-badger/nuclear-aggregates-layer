using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class CalculateOrderPositionPricesRequest : Request
    {
        public long OrderId { get; set; }
        public int Amount { get; set; }
        public decimal Cost { get; set; }
        public decimal CategoryRate { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
    }
}
