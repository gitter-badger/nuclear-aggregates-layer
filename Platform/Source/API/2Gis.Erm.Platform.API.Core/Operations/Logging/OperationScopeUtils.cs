using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public static class OperationScopeUtils
    {
        public static IOperationScope ApplyChanges<TEntity>(this IOperationScope scope, EntityChangesContext changesContext)
            where TEntity : class, IEntity
        {
            IReadOnlyDictionary<ChangesType, IReadOnlyDictionary<long, int>> detectedChanges;
            return scope.ApplyChanges<TEntity>(changesContext, out detectedChanges);
        }

        public static IOperationScope ApplyChanges<TEntity>(
            this IOperationScope scope,
            EntityChangesContext changesContext,
            out IReadOnlyDictionary<ChangesType, IReadOnlyDictionary<long, int>> detectedChanges)
            where TEntity : class, IEntity
        {
            var entityType = typeof(TEntity);

            ConcurrentDictionary<long, int> addedFirms;
            if (changesContext.AddedChanges.TryGetValue(entityType, out addedFirms))
            {
                scope.Added<TEntity>(addedFirms.Keys);
            }

            ConcurrentDictionary<long, int> updatedFirms;
            if (changesContext.UpdatedChanges.TryGetValue(typeof(TEntity), out updatedFirms))
            {
                scope.Updated<TEntity>(updatedFirms.Keys);
            }

            ConcurrentDictionary<long, int> deletedFirms;
            if (changesContext.DeletedChanges.TryGetValue(typeof(TEntity), out deletedFirms))
            {
                scope.Deleted<TEntity>(deletedFirms.Keys);
            }

            detectedChanges = new Dictionary<ChangesType, IReadOnlyDictionary<long, int>>
                        {
                            { 
                                ChangesType.Added, 
                                addedFirms != null ? (IReadOnlyDictionary<long, int>)new ReadOnlyDictionary<long, int>(addedFirms) : new Dictionary<long, int>() 
                            },
                            {
                                ChangesType.Updated, 
                                updatedFirms != null ? (IReadOnlyDictionary<long, int>)new ReadOnlyDictionary<long, int>(updatedFirms) : new Dictionary<long, int>()
                            },
                            {
                                ChangesType.Deleted, 
                                deletedFirms != null ? (IReadOnlyDictionary<long, int>)new ReadOnlyDictionary<long, int>(deletedFirms) : new Dictionary<long, int>()
                            },
                        };

            return scope;
        }

        public static IOperationScope Added<TEntity>(this IOperationScope scope, TEntity changedEntity, params TEntity[] changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Added<TEntity>(changedEntity.Id, ToIds(changedEntities));
        }

        public static IOperationScope Added<TEntity>(this IOperationScope scope, IEnumerable<TEntity> changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Added<TEntity>(ToIds(changedEntities));
        }

        public static IOperationScope Deleted<TEntity>(this IOperationScope scope, TEntity changedEntity, params TEntity[] changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Deleted<TEntity>(changedEntity.Id, ToIds(changedEntities));
        }

        public static IOperationScope Deleted<TEntity>(this IOperationScope scope, IEnumerable<TEntity> changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Deleted<TEntity>(ToIds(changedEntities));
        }

        public static IOperationScope Updated<TEntity>(this IOperationScope scope, TEntity changedEntity, params TEntity[] changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Updated<TEntity>(changedEntity.Id, ToIds(changedEntities));
        }

        public static IOperationScope Updated<TEntity>(this IOperationScope scope, IEnumerable<TEntity> changedEntities)
            where TEntity : class, IEntity, IEntityKey
        {
            return scope.Updated<TEntity>(ToIds(changedEntities));
        }

        private static long[] ToIds(IEnumerable<IEntityKey> entities)
        {
            return entities.Select(x => x.Id).ToArray();
        }
    }
}
