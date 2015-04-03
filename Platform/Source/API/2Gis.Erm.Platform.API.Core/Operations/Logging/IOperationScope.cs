using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationScope : IDisposable
    {
        Guid Id { get; }
        bool IsRoot { get; }
        bool Completed { get; }
        bool IsDisposed { get; }
        StrictOperationIdentity OperationIdentity { get; }

        IOperationScope Added<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity;
        IOperationScope Added<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        IOperationScope Deleted<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity;
        IOperationScope Deleted<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        IOperationScope Updated<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity;
        IOperationScope Updated<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        IOperationScope Complete();
    }
}
