using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    internal static class ActivityExtensions
    {
        public static IEnumerable<TEntityReference> ReferencesIfAny<TEntity, TEntityReference>(this TEntity entity, IEnumerable<EntityReference> references)
            where TEntity : class, IEntity, IEntityKey
            where TEntityReference : EntityReference<TEntity>, new()
        {
            return from reference in (references ?? Enumerable.Empty<EntityReference>())
                   where reference.Id.HasValue
                   select entity.ReferencesIfAny<TEntity, TEntityReference>(reference);
        }

        public static TEntityReference ReferencesIfAny<TEntity, TEntityReference>(this TEntity entity, EntityReference reference)
            where TEntity : class, IEntity, IEntityKey
            where TEntityReference : EntityReference<TEntity>, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return reference == null || !reference.Id.HasValue
                   ? null
                   : new TEntityReference { SourceEntityId = entity.Id, TargetEntityTypeId = reference.EntityTypeId, TargetEntityId = reference.Id.Value };
        }

        public static bool HasReferenceInReserve(this IEnumerable<EntityReference> references, EntityName entityName, Predicate<long> validator)
        {
            return references.Any(s => s.EntityName == entityName && s.Id.HasValue && validator(s.Id.Value));
        }
    }
}