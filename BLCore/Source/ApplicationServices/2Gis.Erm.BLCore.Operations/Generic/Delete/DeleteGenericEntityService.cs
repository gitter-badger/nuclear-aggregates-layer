using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteGenericEntityService<TEntity> : IDeleteGenericEntityService<TEntity>
        where TEntity : class, IEntity, IEntityKey
    {
        private readonly IDeleteAggregateRepository<TEntity> _deleteAggregateRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public DeleteGenericEntityService(
            IDeleteAggregateRepository<TEntity> deleteAggregateRepository,
            IOperationScopeFactory operationScopeFactory)
        {
            _deleteAggregateRepository = deleteAggregateRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, TEntity>())
            {
                _deleteAggregateRepository.Delete(entityId);

                operationScope.Deleted<TEntity>(entityId);
                operationScope.Complete();

                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            throw new NotSupportedException("GetConfirmation is not supported by DeleteGenericEntityService. It is need to implement specific delete service for entity, see IDeleteEntityService");
        }
    }
}
