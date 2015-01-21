using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    public sealed class NullPersistenceChangesRegistry : IPersistenceChangesRegistry
    {
        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> AddedChanges 
        {
            get { return Enumerable.Empty<KeyValuePair<Type, ConcurrentDictionary<long, int>>>(); }
        }

        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> DeletedChanges 
        {
            get { return Enumerable.Empty<KeyValuePair<Type, ConcurrentDictionary<long, int>>>(); }
        }

        public IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> UpdatedChanges
        {
            get { return Enumerable.Empty<KeyValuePair<Type, ConcurrentDictionary<long, int>>>(); }
        }

        public void Added<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
        }

        public void Deleted<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
        }

        public void Updated<TEntity>(params long[] changedEntities) where TEntity : class, IEntity
        {
        }
    }
}