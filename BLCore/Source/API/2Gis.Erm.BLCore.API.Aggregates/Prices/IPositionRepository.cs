using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices
{
    public interface IPositionRepository : IAggregateRootService<Position>,
                                           IActivateAggregateRepository<Position>,
                                           IDeactivateAggregateRepository<Position>,
                                           IDeleteAggregateRepository<Position>,
                                           IDeleteAggregateRepository<PositionChildren>,
                                           IDeleteAggregateRepository<PositionCategory>
    {
        int Delete(PositionCategory positionCategory);
        int DeleteWithSubentities(Position position);

        CategoryWithPositionsDto GetCategoryWithPositions(long entityId);
        IReadOnlyCollection<string> GetMasterPositionNames(Position position);

        bool IsReadOnlyAdvertisementTemplate(long positionId);
        bool IsInPublishedPrices(long positionId);
        Position GetPosition(long positionId);

        void CreateOrUpdate(Position position);
        void CreateOrUpdate(PositionChildren position);
        void CreateOrUpdate(PositionCategory category);
    }
}