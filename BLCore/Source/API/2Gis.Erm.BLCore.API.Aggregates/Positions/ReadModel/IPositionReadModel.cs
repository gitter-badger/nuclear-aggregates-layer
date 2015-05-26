using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel
{
    public interface IPositionReadModel : IAggregateReadModel<Position>
    {
        bool IsSupportedByExport(long positionId);
        IReadOnlyDictionary<PlatformEnum, long> GetPlatformsDictionary(IEnumerable<long> platformDgppIds);
        IEnumerable<LinkingObjectsSchemaPositionDto> GetPositionBindingObjectsInfo(bool isPricePositionComposite, long positionId);
        IReadOnlyCollection<long> GetDependedByPositionOrderIds(long positionId);
        IDictionary<long, PositionsGroup> GetPositionGroups(IEnumerable<long> positionIds);
        IReadOnlyDictionary<long, PositionBindingObjectType> GetPositionBindingObjectTypes(IEnumerable<long> positionIds);
        IReadOnlyDictionary<long, string> GetPositionNames(IEnumerable<long> positionIds);
        IEnumerable<PositionSortingOrderDto> GetPositionsSortingOrder();
        IEnumerable<Position> GetPositions(IEnumerable<long> ids);
        IReadOnlyCollection<long> GetChildPositionIds(long positionId);
        bool KeepCategoriesSynced(long positionId);
        bool AllSubpositionsMustBePicked(long positionId);
        bool AutoCheckPositionsWithFirmBindingType(long positionId);
        PositionSalesModelAndCompositenessDto GetPositionSalesModelAndCompositenessByPricePosition(long pricePositionId);
    }
}