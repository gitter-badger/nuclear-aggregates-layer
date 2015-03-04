using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
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

        public static EntityReference ToReference(this Client client)
        {
            return new EntityReference { EntityName = EntityName.Client, Name = client.Name, Id = client.Id };
        }

        public static IEnumerable<EntityReference> ResolveReferemceAmbiguity<TEntity>(this IEnumerable<TEntity> entities, Func<TEntity, EntityReference> convertToEntityReference)
            where TEntity : IEntity
        {
            var ambiguousEntity = new EntityReference { EntityName = typeof(TEntity).AsEntityName() };
            var firstEntityOrNull =
                (entities ?? Enumerable.Empty<TEntity>()).Select((x, i) => new { Entity = i == 0 ? convertToEntityReference(x) : ambiguousEntity, Index = i })
                                                         .Take(2)
                                                         .LastOrDefault();

            if (firstEntityOrNull != null)
            {
                yield return firstEntityOrNull.Entity;
            }
        }
    }
}
