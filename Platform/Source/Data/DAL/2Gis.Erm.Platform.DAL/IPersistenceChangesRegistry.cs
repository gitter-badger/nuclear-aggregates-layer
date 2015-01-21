using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IPersistenceChangesRegistry
    {
        IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> AddedChanges { get; }
        IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> DeletedChanges { get; }
        IEnumerable<KeyValuePair<Type, ConcurrentDictionary<long, int>>> UpdatedChanges { get; }

        void Added<TEntity>(params long[] changedEntities) where TEntity : class, IEntity;
        void Deleted<TEntity>(params long[] changedEntities) where TEntity : class, IEntity;
        void Updated<TEntity>(params long[] changedEntities) where TEntity : class, IEntity;
    }
}