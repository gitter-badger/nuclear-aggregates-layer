using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class CalculatePositionPricesWithDiscountDistributionRequest : Request
    {
        public int[] Amounts { get; set; }
        public decimal[] Costs { get; set; }
        public decimal[] CategoryRates { get; set; }

        public long OrderId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
    }
}
