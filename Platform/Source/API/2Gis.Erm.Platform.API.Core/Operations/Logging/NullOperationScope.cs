using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class NullOperationScope : IOperationScope
    {
        public NullOperationScope(bool isRootScope, StrictOperationIdentity strictOperationIdentity)
        {
            Id = Guid.NewGuid();
            IsRoot = isRootScope;
            OperationIdentity = strictOperationIdentity;
        }

        public Guid Id { get; private set; }
        public bool IsRoot { get; private set; }
        public bool Completed { get; private set; }
        public bool IsDisposed { get; private set; }
        public StrictOperationIdentity OperationIdentity { get; private set; }

        public IOperationScope Added<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Added<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Deleted<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Deleted<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Updated<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Updated<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            return this;
        }

        public IOperationScope Complete()
        {
            Completed = true;
            return this;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}