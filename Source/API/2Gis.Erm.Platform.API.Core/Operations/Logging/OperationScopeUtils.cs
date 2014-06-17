using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    // TODO {all, 05.06.2014}: При переходе на ServiceBus и логированию diff'ов нижеследующие методы, вероятно, станут частью контракта IOperationScope 
    public static class OperationScopeUtils
    {
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