using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    public static class EntityToEntityReferenceExtensions
    {
        public static EntityReference Lookup<T>(this IEnumerable<EntityReference<T>> references, EntityName entityName, Func<long, string> getName)
            where T : IEntity
        {
            return (references ?? Enumerable.Empty<EntityReference<T>>())
                .Where(x => x.TargetEntityName == entityName)
                .Select(x => new EntityReference { Id = x.TargetEntityId, Name = getName(x.TargetEntityId) })
                .SingleOrDefault();
        }
    }
}