using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class EntityChangesContext
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<long, int>> _addedStorage =
            new ConcurrentDictionary<Type, ConcurrentDictionary<long, int>>();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<long, int>> _deletedStorage =
            new ConcurrentDictionary<Type, ConcurrentDictionary<long, int>>();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<long, int>> _updatedStorage =
            new ConcurrentDictionary<Type, ConcurrentDictionary<long, int>>();

        private readonly Dictionary<ChangesType, ConcurrentDictionary<Type, ConcurrentDictionary<long, int>>> _storages;

        public EntityChangesContext()
        {
            _storages = new Dictionary<ChangesType, ConcurrentDictionary<Type, ConcurrentDictionary<long, int>>>
                {
                    { ChangesType.Added, _addedStorage },
                    { ChangesType.Updated, _updatedStorage },
                    { ChangesType.Deleted, _deletedStorage }
                };
        }

        public IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>> AddedChanges
        {
            get { return new ReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>(_addedStorage); }
        }

        public IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>> DeletedChanges
        {
            get { return new ReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>(_deletedStorage); }
        }

        public IReadOnlyDictionary<Type, ConcurrentDictionary<long, int>> UpdatedChanges
        {
            get { return new ReadOnlyDictionary<Type, ConcurrentDictionary<long, int>>(_updatedStorage); }
        }

        public EntityChangesContext Added<TEntity>(IEnumerable<long> changedEntities) 
            where TEntity : class, IEntity
        {
            return AddChanges2Storage(ChangesType.Added, typeof(TEntity), changedEntities);
        }

        public EntityChangesContext Added(Type entityType, IEnumerable<long> changedEntities)
        {
            if (!entityType.IsEntity())
            {
                throw new ArgumentException("Specified entity type " + entityType + " doesn't implement " + typeof(IEntity)); 
            }

            return AddChanges2Storage(ChangesType.Added, entityType, changedEntities);
        }

        public EntityChangesContext Deleted<TEntity>(IEnumerable<long> changedEntities)
            where TEntity : class, IEntity
        {
            return AddChanges2Storage(ChangesType.Deleted, typeof(TEntity), changedEntities);
        }

        public EntityChangesContext Deleted(Type entityType, IEnumerable<long> changedEntities) 
        {
            if (!entityType.IsEntity())
            {
                throw new ArgumentException("Specified entity type " + entityType + " doesn't implement " + typeof(IEntity));
            }

            return AddChanges2Storage(ChangesType.Deleted, entityType, changedEntities);
        }

        public EntityChangesContext Updated<TEntity>(IEnumerable<long> changedEntities) 
            where TEntity : class, IEntity
        {
            return AddChanges2Storage(ChangesType.Updated, typeof(TEntity), changedEntities);
        }
        
        public EntityChangesContext Updated(Type entityType, IEnumerable<long> changedEntities) 
        {
            if (!entityType.IsEntity())
            {
                throw new ArgumentException("Specified entity type " + entityType + " doesn't implement " + typeof(IEntity));
            }

            return AddChanges2Storage(ChangesType.Updated, entityType, changedEntities);
        }

        private static ConcurrentDictionary<long, int> AddToDictionary(ConcurrentDictionary<long, int> dictionary, long changedEntity)
        {
            dictionary.AddOrUpdate(changedEntity, 1, (key, oldValue) => oldValue + 1);
            return dictionary;
        }

        private EntityChangesContext AddChanges2Storage(ChangesType changesType, Type entityType, IEnumerable<long> changedEntities)
        {
            foreach (var changedEntity in changedEntities)
            {
                _storages[changesType].AddOrUpdate(entityType,
                                                   new ConcurrentDictionary<long, int>(new[] { new KeyValuePair<long, int>(changedEntity, 1) }),
                                                   (type, dictionary) => AddToDictionary(dictionary, changedEntity));
            }

            return this;
        }
    }
}
