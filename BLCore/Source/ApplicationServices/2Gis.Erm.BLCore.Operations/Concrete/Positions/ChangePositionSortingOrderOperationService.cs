using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Positions
{
    public sealed class ChangePositionSortingOrderOperationService : IChangePositionSortingOrderOperationService
    {
        private readonly IPositionReadModel _readModel;
        private readonly IChangePositionSortingOrderAggregateService _aggregateService;

        public ChangePositionSortingOrderOperationService(IPositionReadModel readModel, IChangePositionSortingOrderAggregateService aggregateService)
        {
            _readModel = readModel;
            _aggregateService = aggregateService;
        }

        public void ApplyChanges(PositionSortingOrderDto[] data)
        {
            var positions = _readModel.GetPositions(data.Select(dto => dto.Id));
            _aggregateService.ChangeSorting(positions, data);
        }
    }
}