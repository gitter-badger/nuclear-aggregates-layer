using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    internal static class ActivityUtilityExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the entity could be treated as a regarding object.
        /// </summary>
        public static bool CanBeRegardingObject(this IEntityType entityName)
        {
            return entityName.Equals(EntityType.Instance.Client()) || entityName.Equals(EntityType.Instance.Firm()) || entityName.Equals(EntityType.Instance.Deal());
        }

        /// <summary>
        /// Returns a value indicating whether the entity could be treated as an attendee.
        /// </summary>
        public static bool CanBeContacted(this IEntityType entityName)
        {
            return entityName.Equals(EntityType.Instance.Contact());
        }

        public static bool IsActivity(this IEntityType entityName)
        {
            return entityName.Equals(EntityType.Instance.Appointment()) ||
                   entityName.Equals(EntityType.Instance.Letter()) ||
                   entityName.Equals(EntityType.Instance.Phonecall()) ||
                   entityName.Equals(EntityType.Instance.Task());
        }
        
        public static EntityReference ToEntityReference<TEntity>(this TEntity entity, Func<TEntity, string> getName = null)
            where TEntity : IEntity, IEntityKey
        {
            return new EntityReference
                       {
                           EntityTypeId = typeof(TEntity).AsEntityName().Id, 
                           Id = entity.Id,
                           Name = getName != null ? getName(entity) : null, 
                       };
        }

        public static EntityReference ToEntityReference<TEntity>(this EntityReference<TEntity> reference)
            where TEntity : IEntity
        {
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }

            return new EntityReference
            {
                Id = reference.TargetEntityId,
                EntityTypeId = reference.TargetEntityTypeId,
            };
        }

        public static IEnumerable<EntityReference> ToEntityReferences<TEntity>(this EntityReference<TEntity> reference)
            where TEntity : IEntity
        {
            return new[] { reference }.Where(x => x != null).ToEntityReferences();
        }

        public static IEnumerable<EntityReference> ToEntityReferences<TEntity>(this IEnumerable<EntityReference<TEntity>> references)
            where TEntity : IEntity
        {
            return references.Select(ToEntityReference);
        }

        public static IEnumerable<EntityReference> ToEntityReferencesWithNoAmbiguity<TEntity>(this IEnumerable<TEntity> entities, Func<TEntity, string> getName)
            where TEntity : IEntity, IEntityKey
        {
            var ambiguousEntity = new EntityReference { EntityTypeId = typeof(TEntity).AsEntityName().Id };
            var firstOrAmbiguous = (entities ?? Enumerable.Empty<TEntity>())
                .Select((x, i) => new 
                    {
                        Reference = i == 0 ? new EntityReference { EntityTypeId = typeof(TEntity).AsEntityName().Id, Id = x.Id,  Name = getName(x) } : ambiguousEntity,
                        Index = i
                    })
                .Take(2)
                .LastOrDefault();

            if (firstOrAmbiguous != null)
            {
                yield return firstOrAmbiguous.Reference;
            }
        }      
    }
}
