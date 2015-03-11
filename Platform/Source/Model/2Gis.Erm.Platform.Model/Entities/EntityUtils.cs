using System;

using DoubleGis.Erm.Platform.Common.Utils.Data;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntityUtils
    {
        public static bool IsNew(this IEntityKey entity)
        {
            var stateTrackingEntity = entity as IStateTrackingEntity;
            if (stateTrackingEntity != null)
            {
                return stateTrackingEntity.Timestamp == null;
            }

            if (entity.GetType().IsInstanceShared())
            {
                throw new InvalidOperationException(string.Format("Cannot check if non-statetracking entity {0} is new since its Id is managed by user", entity.GetType().Name));
            }

            return entity.Id == 0;
        }

        public static long GetId<TEntity>(this TEntity entity)
            where TEntity : class, IEntity
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null)
            {
                return entityWithId.Id;
            }

            throw new InvalidOperationException(string.Format("Can't extract Id for entity {0}. Valid domain entity must implement {1}",
                                                              typeof(TEntity).Name,
                                                              typeof(IEntityKey).Name));
        }

        public static TEntity ResetToNew<TEntity>(this TEntity entity)
            where TEntity : class, IEntity
        {
            var entityWithId = entity as IEntityKey;
            if (entityWithId != null)
            {
                entityWithId.Id = 0;
            }

            var stateTrackingEntity = entity as IStateTrackingEntity;
            if (stateTrackingEntity != null)
            {
                stateTrackingEntity.Timestamp = null;
            }

            var replicableEntity = entity as IReplicableEntity;
            if (replicableEntity != null)
            {
                replicableEntity.ReplicationCode = Guid.Empty;
            }

            return entity;
        }

        public static bool SameVersionAs(this IStateTrackingEntity entity1, IStateTrackingEntity entity2)
        {
            return entity1.Timestamp != null && entity1.Timestamp.SameAs(entity2.Timestamp);
        }

        public static bool IsPartable(this IEntityKey entity)
        {
            return entity is IPartable;
        }

        public static bool IsPartable(this Type entityType)
        {
            return typeof(IPartable).IsAssignableFrom(entityType);
        }
    }
}