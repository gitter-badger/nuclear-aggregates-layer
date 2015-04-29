using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    public interface IPriceReadModel : IAggregateReadModel<Price>
    {
        Price GetPrice(long priceId);
        PricePosition GetPricePosition(long pricePositionId);
        PricePositionRateType GetPricePositionRateType(long pricePositionId);
        bool IsDifferentPriceExistsForDate(long priceId, long organizationUnitId, DateTime beginDate);
        PriceValidationDto GetPriceValidationDto(long priceId);
        long GetActualPriceId(long organizationUnitId);
        bool IsPriceActive(long priceId);
        bool IsPriceExist(long priceId);
        bool IsPriceLinked(long priceId);
        bool IsPricePublished(long priceId);
        bool IsPriceContainsPosition(long priceId, long positionId);
        bool IsPriceContainsPositionWithinNonDeleted(long priceId, long positionId);
        bool IsPricePositionExist(long priceId, long positionId, long pricePositionId);
        bool IsPricePositionExistWithinNonDeleted(long priceId, long positionId, long pricePositionId);
        AllPriceDescendantsDto GetAllPriceDescendantsDto(long priceId);
        AllPricePositionDescendantsDto GetAllPricePositionDescendantsDto(long pricePositionId, long positionId);
        PriceDto GetPriceDto(long priceId);
        long GetPriceId(long pricePositionId);
        decimal GetCategoryRateByFirm(long firmId);
        decimal GetCategoryRateByCategory(long[] categoryId, long organizationUnitId);
        decimal GetPricePositionCost(long pricePositionId);
        PricePosition GetPricePosition(long priceId, long positionId);
        PricePositionDetailedInfo GetPricePositionDetailedInfo(long pricePositionId);
    }
}