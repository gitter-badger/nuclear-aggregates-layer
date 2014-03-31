using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel
{
    public interface IPriceReadModel : IAggregateReadModel<Price>
    {
        PricePositionRateType GetPricePositionRateType(long pricePositionId);
        decimal GetCategoryRateByFirm(long firmId);
        decimal GetCategoryRateByCategory(long categoryId, long organizationUnitId);
        decimal GetPricePositionCost(long pricePositionId);
    }
}