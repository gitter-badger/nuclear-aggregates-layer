using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices
{
    public interface IPriceRepository : IAggregateRootRepository<Price>,
                                        IActivateAggregateRepository<AssociatedPositionsGroup>,
                                        IDeactivateAggregateRepository<AssociatedPositionsGroup>,
                                        IDeactivateAggregateRepository<AssociatedPosition>
    {
        int Activate(AssociatedPositionsGroup associatedPositionsGroup);
        int Deactivate(AssociatedPositionsGroup associatedPositionsGroup);
        int Deactivate(AssociatedPosition associatedPosition);

        IEnumerable<PricePositionDto> GetPricePositions(IEnumerable<long> requiredPriceIds, IEnumerable<long> requiredPositionIds);

        bool TryGetActualPriceId(long organizationUnitId, DateTime date, out long actualPriceId);

        void Publish(Price price, long organizationUnitId, DateTime beginDate, DateTime publishDate);
        void Unpublish(Price price);

        [Obsolete]
        void CreateOrUpdate(AssociatedPosition associatedPosition);

        [Obsolete]
        void CreateOrUpdate(AssociatedPositionsGroup associatedPosition);
    }
}