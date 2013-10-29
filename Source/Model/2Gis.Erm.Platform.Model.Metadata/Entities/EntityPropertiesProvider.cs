using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public sealed class EntityPropertiesProvider : IEntityPropertiesProvider
    {
        private readonly IReadOnlyDictionary<EntityName, IEnumerable<EntityProperty>> _entityPropertiesMap = EntityProperties.Settings;

        public IEnumerable<EntityProperty> GetProperties(EntityName entity)
        {
            if (!_entityPropertiesMap.ContainsKey(entity))
            {
                throw new ArgumentException(string.Format("Для {0} нет набора свойств", entity), "entity");
            }

            return _entityPropertiesMap[entity];
        }

        public IReadOnlyDictionary<EntityName, IEnumerable<EntityProperty>> EntityPropertiesMap
        {
            get
            {
                return _entityPropertiesMap;
            }
        }
    }
}
