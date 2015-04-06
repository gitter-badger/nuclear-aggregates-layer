using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Position;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.Operations
{
    public interface IChangePositionSortingOrderAggregateService : IAggregateSpecificOperation<Position, ChangePositionSortingOrderIdentity>
    {
        void ChangeSorting(IEnumerable<Position> positions, IEnumerable<PositionSortingOrderDto> sorting);
    }
}
