using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public interface IPositionRepository : IAggregateRootRepository<Position>,
                                           IActivateAggregateRepository<Position>,
                                           IDeactivateAggregateRepository<Position>,
                                           IDeleteAggregateRepository<Position>,
                                           IDeleteAggregateRepository<PositionChildren>,
                                           IDeleteAggregateRepository<PositionCategory>
    {
        int Activate(Position position);
        int Deactivate(Position position);

        int Delete(Position position);
        int Delete(PositionChildren link);
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
