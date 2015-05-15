using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Ambivalent
{
    /// <summary>
    /// Сущности, которым пока что позволительно участвовать и в аггрегирующих репозиториях и в упрощенных.
    /// Необходимо стремиться к опустошению этого списка.
    /// </summary>
    public static class AmbivalentEntities
    {
        public static readonly IEntityType[] Entities =
        { 
            EntityType.Instance.FileWithContent()
        };

        public static bool IsAmbivalent(this IEntityType entityName)
        {
            return Entities.Contains(entityName);
        }

        public static bool IsAmbivalent(this Type entityType)
        {
            IEntityType entityName;
            if (!entityType.TryGetEntityName(out entityName))
            {
                return false;
            }

            return entityName.IsAmbivalent();
        }
    }
}
