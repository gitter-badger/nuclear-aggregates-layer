using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    internal static class ActivityUtilityExtensions
    {
        /// <summary>
        /// Returns a value indicating whether the entity could be treated as a regarding object.
        /// </summary>
        public static bool CanBeRegardingObject(this EntityName entityName)
        {
            return entityName == EntityName.Client || entityName == EntityName.Firm || entityName == EntityName.Deal;
        }

        /// <summary>
        /// Returns a value indicating whether the entity could be treated as an attendee.
        /// </summary>
        public static bool CanBeContacted(this EntityName entityName)
        {
            return entityName == EntityName.Contact;
        }

        public static bool IsActivity(this EntityName entityName)
        {
            return entityName == EntityName.Appointment || entityName == EntityName.Letter || entityName == EntityName.Phonecall || entityName == EntityName.Task;
        }

        public static IEnumerable<T> LookupElements<T>(this IReadOnlyDictionary<EntityName, Func<long, IEnumerable<T>>> lookups, EntityName entityName, long? entityId)
        {
            Func<long, IEnumerable<T>> lookup;
            if (entityId != null && lookups.TryGetValue(entityName, out lookup))
            {
                return lookup(entityId.Value);
            }

            return Enumerable.Empty<T>();
        }

        public static EntityReference ToEntityReference<TEntity>(this TEntity entity, Func<TEntity, string> getName = null)
            where TEntity : IEntity, IEntityKey
        {
            return new EntityReference
                       {
                           EntityName = typeof(TEntity).AsEntityName(), 
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
                EntityName = reference.TargetEntityName,
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
            var ambiguousEntity = new EntityReference { EntityName = typeof(TEntity).AsEntityName() };
            var firstOrAmbiguous = (entities ?? Enumerable.Empty<TEntity>())
                .Select((x, i) => new 
                    {
                        Reference = i == 0 ? new EntityReference { EntityName = typeof(TEntity).AsEntityName(), Id = x.Id,  Name = getName(x) } : ambiguousEntity,
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
