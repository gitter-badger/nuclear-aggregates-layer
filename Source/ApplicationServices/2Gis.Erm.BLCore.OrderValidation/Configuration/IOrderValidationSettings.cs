using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Prices;

namespace DoubleGis.Erm.BLCore.OrderValidation.Configuration
{
    public interface IOrderValidationSettings
    {
        IDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalDeniedPositions(IEnumerable<long> requiredPositionIds);
        IDictionary<long, IEnumerable<PricePositionDto.RelatedItemDto>> GetGlobalAssociatedPositions(IEnumerable<long> requiredPositionIds);
    }
}
