using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
                   : new TEntityReference { SourceEntityId = entity.Id, TargetEntityName = reference.EntityName, TargetEntityId = reference.Id.Value };
        }

        public static bool IsAnyReferencedFirmInReserve(this IFirmReadModel firmReadModel, IEnumerable<EntityReference> references)
        {
            return references.Any(s => s.EntityName == EntityName.Firm && s.Id.HasValue && firmReadModel.IsFirmInReserve(s.Id.Value));
        }

        public static bool IsAnyReferencedClientInReserve(this IClientReadModel firmReadModel, IEnumerable<EntityReference> references)
        {
            return references.Any(s => s.EntityName == EntityName.Client && s.Id.HasValue && firmReadModel.IsClientInReserve(s.Id.Value));
        }
    }
}