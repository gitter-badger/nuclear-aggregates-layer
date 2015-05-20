using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public sealed class DeactivateGenericEntityOperationService<TEntity> : IDeactivateGenericEntityService<TEntity> where TEntity : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        private readonly IDeactivateAggregateRepository<TEntity> _deactivateAggregateRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeactivateGenericEntityOperationService(
            IDeactivateAggregateRepository<TEntity> deactivateAggregateRepository,
            IOperationScopeFactory operationScopeFactory)
        {
            _deactivateAggregateRepository = deactivateAggregateRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, TEntity>())
            {
                _deactivateAggregateRepository.Deactivate(entityId);
                operationScope.Updated<TEntity>(entityId);
                operationScope.Complete();
                return null;
            }
        }
    }
}