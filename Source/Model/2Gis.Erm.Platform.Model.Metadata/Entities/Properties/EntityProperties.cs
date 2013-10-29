﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties
{
    public static partial class EntityProperties
    {
        private static readonly Dictionary<EntityName, IEnumerable<EntityProperty>> EntityPropertiesMap = new Dictionary<EntityName, IEnumerable<EntityProperty>>();

        static EntityProperties()
        {
            typeof(EntityProperties).Extract<IEnumerable<EntityProperty>>(EntityPropertyProcessor);
        }

        public static IReadOnlyDictionary<EntityName, IEnumerable<EntityProperty>> Settings
        {
            get
            {
                return EntityPropertiesMap;
            }
        }

        private static void EntityPropertyProcessor(IEnumerable<EntityProperty> entityProperties)
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
