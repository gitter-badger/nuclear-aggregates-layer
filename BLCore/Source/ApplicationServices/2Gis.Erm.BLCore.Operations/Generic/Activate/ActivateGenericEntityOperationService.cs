using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public sealed class ActivateGenericEntityOperationService<TEntity> : IActivateGenericEntityService<TEntity>
        where TEntity : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        private readonly IActivateAggregateRepository<TEntity> _activateAggregateRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ActivateGenericEntityOperationService(IActivateAggregateRepository<TEntity> activateAggregateRepository, IOperationScopeFactory operationScopeFactory)
        {
            _activateAggregateRepository = activateAggregateRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, TEntity>())
            {
                var result = _activateAggregateRepository.Activate(entityId);
                operationScope.Updated<TEntity>(entityId);
                operationScope.Complete();

                return result;
            }
        }
    }
}
