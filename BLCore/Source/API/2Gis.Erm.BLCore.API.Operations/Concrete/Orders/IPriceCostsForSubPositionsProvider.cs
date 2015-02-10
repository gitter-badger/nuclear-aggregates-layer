using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Common.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IPriceCostsForSubPositionsProvider
    {
        IReadOnlyCollection<PriceCostDto> GetPriceCostsForSubPositions(long parentPositionId, long priceId);
    }

    public sealed class PriceCostDto
    {
        public long PositionId { get; set; }
        public PlatformEnum Platform { get; set; }
        public decimal Cost { get; set; }
    }
}
