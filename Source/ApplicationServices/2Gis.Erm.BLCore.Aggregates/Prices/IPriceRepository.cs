using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public interface IPriceRepository : IAggregateRootRepository<Price>,
                                       IActivateAggregateRepository<AssociatedPositionsGroup>,
                                       IActivateAggregateRepository<DeniedPosition>,
                                       IActivateAggregateRepository<PricePosition>,
                                       IActivateAggregateRepository<Price>,
                                       IDeactivateAggregateRepository<Price>,
                                       IDeactivateAggregateRepository<PricePosition>,
                                       IDeactivateAggregateRepository<AssociatedPositionsGroup>,
                                       IDeactivateAggregateRepository<AssociatedPosition>,
                                       IDeactivateAggregateRepository<DeniedPosition>
    {
        int Activate(AssociatedPositionsGroup associatedPositionsGroup);
        int Activate(DeniedPosition deniedPosition);
        int Activate(PricePosition pricePosition);
        int Activate(Price price);
        int Add(Price price);
        int Add(PricePosition pricePosition);
        int Add(AssociatedPositionsGroup associatedPositionsGroup);
        int Add(AssociatedPosition associatedPosition);
        int Add(DeniedPosition deniedPosition);
        int Update(Price price);
        int Deactivate(Price price);
        int Deactivate(PricePosition pricePosition);
        int Deactivate(AssociatedPositionsGroup associatedPositionsGroup);
        int Deactivate(AssociatedPosition associatedPosition);
        int Deactivate(DeniedPosition associatedPosition);

        void DeleteWithSubentities(long entityId);

        void CheckPriceBusinessRules(long priceId, long organizationUnitId, DateTime? beginDate, DateTime? publishDate);
        long GetCurrencyId(long organizationUnitId);
        PriceDto GetPriceDto(long priceId);

        bool PriceExists(long priceId);
        bool PricePublishedForToday(long priceId);
        bool PriceContainsPosition(long priceId, long positionId);
        IEnumerable<PricePositionDto> GetPricePositions(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds);

        PricePosition GetPricePosition(long pricePositionId);
        PriceToCopyDto GetPriceToCopyDto(long sourcePriceId);
        IEnumerable<DeniedPosition> GetDeniedPositions(long priceId, long positionId);
        IEnumerable<AssociatedPositionsGroup> GetAssociatedPositionsGroups(long pricePositionId);
        IEnumerable<AssociatedPosition> GetAssociatedPositions(long positionsGroupId);

        bool PriceHasLinkedOrders(long entityId);

        long GetPriceOrganizationUnitId(long priceId);

        void CreateOrUpdate(Price price);

        bool TryGetActualPriceId(long organizationUnitId, DateTime date, out long actualPriceId);

        void Publish(long priceId, long organizationUnitId, DateTime beginDate, DateTime publishDate);
        void Unpublish(long priceId);

        void CreateOrUpdate(PricePosition pricePosition);
        void CreateOrUpdate(AssociatedPosition associatedPosition);
        void CreateOrUpdate(DeniedPosition deniedPosition);
        void CreateOrUpdate(AssociatedPositionsGroup associatedPosition);
    }
}