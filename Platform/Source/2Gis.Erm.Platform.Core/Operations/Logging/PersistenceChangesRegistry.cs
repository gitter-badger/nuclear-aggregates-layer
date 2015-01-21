using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class PersistenceChangesRegistry : IPersistenceChangesRegistry
    {
        private readonly EntityChangesContext _entityChangesContext = new EntityChangesContext();

        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> AddedChanges 
        {
            get { return _entityChangesContext.AddedChanges; }
        }

        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> DeletedChanges 
        {
            get { return _entityChangesContext.DeletedChanges; }
        }

        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> UpdatedChanges
        {
            get { return _entityChangesContext.UpdatedChanges; }
        }

        public void Added<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
            _entityChangesContext.Added<TEntity>(changedEntities);
        }

        public void Deleted<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
            _entityChangesContext.Deleted<TEntity>(changedEntities);
        }

        public void Updated<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
            _entityChangesContext.Updated<TEntity>(changedEntities);
        }
    }
}
