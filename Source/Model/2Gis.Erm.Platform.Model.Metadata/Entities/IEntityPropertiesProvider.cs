using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public interface IEntityPropertiesProvider
    {
        IEnumerable<EntityProperty> GetProperties(EntityName entity);
        IReadOnlyDictionary<EntityName, IEnumerable<EntityProperty>> EntityPropertiesMap { get; }
    }
}
