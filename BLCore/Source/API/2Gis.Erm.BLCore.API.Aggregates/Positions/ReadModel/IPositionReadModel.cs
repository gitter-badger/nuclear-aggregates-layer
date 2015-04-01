using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel
{
    public interface IPositionReadModel : IAggregateReadModel<Position>
    {
        PositionBindingObjectType GetPositionBindingObjectType(long positionId);
        bool IsSupportedByExport(long positionId);
        IReadOnlyDictionary<PlatformEnum, long> GetPlatformsDictionary(IEnumerable<long> platformDgppIds);
        string GetPositionName(long positionId);
        Position GetPositionByPricePositionId(long pricePositionId);
        IEnumerable<LinkingObjectsSchemaPositionDto> GetPositionBindingObjectsInfo(bool isPricePositionComposite, long positionId);
        IReadOnlyCollection<long> GetDependedByPositionOrderIds(long positionId);
        IDictionary<long, PositionsGroup> GetPositionGroups(IEnumerable<long> positionIds);
    }
}