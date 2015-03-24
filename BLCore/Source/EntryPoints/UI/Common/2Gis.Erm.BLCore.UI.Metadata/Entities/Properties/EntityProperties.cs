using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        private static readonly Dictionary<IEntityType, IEnumerable<EntityPropertyMetadata>> EntityPropertiesMap = new Dictionary<IEntityType, IEnumerable<EntityPropertyMetadata>>();

        static EntityProperties()
        {
            typeof(EntityProperties).Extract<IEnumerable<EntityPropertyMetadata>>(EntityPropertyProcessor);
        }

        public static IReadOnlyDictionary<IEntityType, IEnumerable<EntityPropertyMetadata>> Settings
        {
            get
            {
                return EntityPropertiesMap;
            }
        }

        private static void EntityPropertyProcessor(IEnumerable<EntityPropertyMetadata> entityProperties)
        {
            var settings = entityProperties.ToArray();
            var indicatorElement = settings.FirstOrDefault();
            if (indicatorElement == null)
            {
                return;
            }

            var domainEntityIndicator = typeof(IDomainEntityDto);
            var domainEntityGenericIndicator = typeof(IDomainEntityDto<>);
            if (indicatorElement.DeclaringType == null)
            {
                return;
            }

            if (!domainEntityIndicator.IsAssignableFrom(indicatorElement.DeclaringType))
            {
                throw new InvalidOperationException("Entity property is not correspond to any valid type marked with " + domainEntityIndicator);
            }

            var entityName =
                indicatorElement.DeclaringType.GetInterfaces()
                    .Where(t => domainEntityIndicator.IsAssignableFrom(t)
                            && (t.IsGenericType && domainEntityGenericIndicator == t.GetGenericTypeDefinition()))
                    .Select(type => type.GetGenericArguments()[0].AsEntityName())
                    .Single();

            EntityPropertiesMap.Add(entityName, settings);
        }
    }
}
