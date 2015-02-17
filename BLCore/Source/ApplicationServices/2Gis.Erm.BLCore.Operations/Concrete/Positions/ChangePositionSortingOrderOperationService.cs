using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Positions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Position;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Positions
{
    public sealed class ChangePositionSortingOrderOperationService : IChangePositionSortingOrderOperationService
    {
        private readonly IPositionReadModel _readModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IChangePositionSortingOrderAggregateService _aggregateService;

        public ChangePositionSortingOrderOperationService(IPositionReadModel readModel, IChangePositionSortingOrderAggregateService aggregateService, IOperationScopeFactory scopeFactory)
        {
            _readModel = readModel;
            _aggregateService = aggregateService;
            _scopeFactory = scopeFactory;
        }

        public void ApplyChanges(PositionSortingOrderDto[] data)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ChangePositionSortingOrderIdentity>())
            {
                var positions = _readModel.GetPositions(data.Select(dto => dto.Id));
                _aggregateService.ChangeSorting(positions, data);
                scope.Updated(positions);
                scope.Complete();
            }
        }
    }
}