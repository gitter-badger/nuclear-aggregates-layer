using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationScopeContextsStorage
    {
        TrackedUseCase UseCase { get; }

        void Added<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        void Deleted<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        void Updated<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
    }
}