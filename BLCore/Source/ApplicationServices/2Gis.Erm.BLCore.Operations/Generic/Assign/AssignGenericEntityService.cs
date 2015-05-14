using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignGenericEntityService<TEntity> : IAssignGenericEntityService<TEntity> 
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        private readonly IAssignAggregateRepository<TEntity> _assignAggregateRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AssignGenericEntityService(IAssignAggregateRepository<TEntity> assignAggregateRepository, IOperationScopeFactory scopeFactory)
        {
            _assignAggregateRepository = assignAggregateRepository;
            _scopeFactory = scopeFactory;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, TEntity>())
            {
                _assignAggregateRepository.Assign(entityId, ownerCode);

                operationScope
                    .Updated<TEntity>(entityId)
                    .Complete();
            }
            
            return null;
        }
    }
}
