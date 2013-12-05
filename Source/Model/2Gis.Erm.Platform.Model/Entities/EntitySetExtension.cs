using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntitySetExtension
    {
        public static EntitySet ToEntitySet(this EntityName entityName)
        {
            return new EntitySet(new[] { entityName });
        }

        public static EntitySet ToEntitySet(this EntityName[] entityNames)
        {
            return new EntitySet(entityNames);
        }

        public static EntitySet Merge(this IEnumerable<EntitySet> descriptors)
        {
            return new EntitySet(descriptors.SelectMany(d => d.Entities).Distinct().ToArray());
        }

        /// <summary>
        /// ќпредел€ет все ли сущности в указанном наборе конкретные, или есть placeholder, 
        /// т.е. fake composite entityname, вместо которых может подставлена люба€ сущность  из некоторого множества 
        /// </summary>
        public static bool IsOpenSet(this EntitySet entitySet)
        {
            return entitySet.Entities.Contains(EntitySet.OpenEntitiesSetIndicator);
        }

        /// <summary>
        /// ¬озвращает последовательность EntitySet, которые покрывают все возможные сочетани€, между сущност€ми, 
        /// которые могут быть подставленны вместо openset placeholders в исходном EntitySet.
        /// ‘актически подбирает все сочетани€, которые могли бы закрыть open entity set (аналогично open -> closed generics)
        /// </summary>
        public static IEnumerable<EntitySet> ToConcreteSets(this EntitySet openEntitySet)
        {
            return new OpenEnitiesSetEnumerator(openEntitySet);
        }
    }
}