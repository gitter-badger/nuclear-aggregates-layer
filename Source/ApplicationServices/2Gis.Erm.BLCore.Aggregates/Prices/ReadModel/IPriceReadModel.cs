using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel
{
    public interface IPriceReadModel : IAggregateReadModel<Price>
    {
        PricePositionRateType GetPricePositionRateType(long pricePositionId);
        decimal GetCategoryRate(long pricePositionId, long firmId, long? categoryId);
    }
}