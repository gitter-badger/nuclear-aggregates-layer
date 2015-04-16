using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivatePositionOperationService : IDeactivateGenericEntityService<Position>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeactivatePositionOperationService(IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, Position>())
            {
                var deactivateAggregateRepository = _positionRepository as IDeactivateAggregateRepository<Position>;
                deactivateAggregateRepository.Deactivate(entityId);

                scope.Updated<Position>(entityId)
                     .Complete();
            }

            return null;
        }
    }
}