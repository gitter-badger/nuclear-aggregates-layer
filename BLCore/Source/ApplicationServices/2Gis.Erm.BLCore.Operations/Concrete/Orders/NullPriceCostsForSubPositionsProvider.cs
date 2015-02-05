using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public sealed class NullPriceCostsForSubPositionsProvider : IPriceCostsForSubPositionsProvider
    {
        public IReadOnlyCollection<PriceCostDto> GetPriceCostsForSubPositions(long parentPositionId, long priceId)
        {
            return new PriceCostDto[0];
        }
    }
}
