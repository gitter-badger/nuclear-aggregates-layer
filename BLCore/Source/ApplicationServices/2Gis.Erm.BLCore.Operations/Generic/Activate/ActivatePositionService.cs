using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivatePositionService : IActivateGenericEntityService<Position>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActivatePositionService(IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<ActivateIdentity, Position>())
            {
                var activateAggregateRepository = _positionRepository as IActivateAggregateRepository<Position>;
                var result = activateAggregateRepository.Activate(entityId);

                scope.Updated<Position>(entityId)
                     .Complete();

                return result;
            }
        }
    }
}