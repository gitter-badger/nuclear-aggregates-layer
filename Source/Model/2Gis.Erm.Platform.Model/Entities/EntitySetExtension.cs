using System.Collections.Generic;
using System.Linq;

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
    }
}