using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices
{
    public interface IPositionRepository : IAggregateRootRepository<Position>,
                                           IActivateAggregateRepository<Position>,
                                           IDeactivateAggregateRepository<Position>,
                                           IDeleteAggregateRepository<Position>,
                                           IDeleteAggregateRepository<PositionChildren>,
                                           IDeleteAggregateRepository<PositionCategory>
    {
        int Delete(PositionCategory positionCategory);
        int DeleteWithSubentities(Position position);

        CategoryWithPositionsDto GetCategoryWithPositions(long entityId);
        string[] GetMasterPositionNames(Position position);

        bool IsReadOnlyAdvertisementTemplate(long positionId);
        bool IsInPublishedPrices(long positionId);
        Position GetPosition(long positionId);

        void CreateOrUpdate(Position position);
        void CreateOrUpdate(PositionChildren position);
        void CreateOrUpdate(PositionCategory category);
    }
}