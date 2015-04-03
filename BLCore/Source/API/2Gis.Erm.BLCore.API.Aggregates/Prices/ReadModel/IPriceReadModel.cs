using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
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
        bool DoesPriceExist(long priceId);
        bool IsPriceLinked(long priceId);
        bool IsPricePublished(long priceId);
        bool DoesPriceContainPosition(long priceId, long positionId);
        bool DoesPriceContainPositionWithinNonDeleted(long priceId, long positionId);
        bool DoesPricePositionExist(long priceId, long positionId, long pricePositionId);
        bool DoesPricePositionExistWithinNonDeleted(long priceId, long positionId, long pricePositionId);
        AllPriceDescendantsDto GetAllPriceDescendantsDto(long priceId);
        AllPricePositionDescendantsDto GetAllPricePositionDescendantsDto(long pricePositionId, long positionId);
        PriceDto GetPriceDto(long priceId);
        long GetPriceId(long pricePositionId);
        decimal GetCategoryRateByFirm(long firmId);
        decimal GetCategoryRateByCategory(long[] categoryId, long organizationUnitId);
        decimal GetPricePositionCost(long pricePositionId);
        PricePosition GetPricePosition(long priceId, long positionId);
        PricePositionDetailedInfo GetPricePositionDetailedInfo(long pricePositionId);
        IEnumerable<DeniedPosition> GetDeniedPositions(long positionId, long positionDeniedId, long priceId);
        IEnumerable<DeniedPosition> GetDeniedPositionsOrSymmetric(long positionId, long positionDeniedId, long priceId);
    }
}