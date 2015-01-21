using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.EAV
{
    public interface IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstace>
        where TEntity : IEntity
        where TEntityInstance : IDynamicEntityInstance
        where TPropertyInstace : IDynamicEntityPropertyInstance
    {
        TEntity ConvertFromDynamicEntityInstance(TEntityInstance dynamicEntityInstance, ICollection<TPropertyInstace> propertyInstances);
        Tuple<TEntityInstance, ICollection<TPropertyInstace>> ConvertToDynamicEntityInstance(TEntity entity, ICollection<TPropertyInstace> propertyInstances, long? referencedEntityId);
    }
}