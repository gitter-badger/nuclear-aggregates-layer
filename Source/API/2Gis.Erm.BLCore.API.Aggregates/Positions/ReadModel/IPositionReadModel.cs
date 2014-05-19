using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel
{
    public interface IPositionReadModel : IAggregateReadModel<Position>
    {
        PositionBindingObjectType GetPositionBindingObjectType(long positionId);
        bool IsSupportedByExport(long positionId);
    }
}