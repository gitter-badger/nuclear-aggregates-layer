using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Ambivalent
{
    /// <summary>
    /// Сущности, которым пока что позволительно участвовать и в аггрегирующих репозиториях и в упрощенных.
    /// Необходимо стремиться к опустошению этого списка.
    /// </summary>
    public static class AmbivalentEntities
    {
        public static readonly EntityName[] Entities = new[] 
        { 
            EntityName.FileWithContent
        };

        public static bool IsAmbivalent(this EntityName entityName)
        {
            return Entities.Contains(entityName);
        }

        public static bool IsAmbivalent(this Type entityType)
        {
            EntityName entityName;
            if (!entityType.TryGetEntityName(out entityName))
            {
                return false;
            }

            return entityName.IsAmbivalent();
        }
    }
}
