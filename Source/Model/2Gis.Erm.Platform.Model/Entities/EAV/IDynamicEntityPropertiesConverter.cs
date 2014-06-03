using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.EAV
{
    // TODO {all, 07.04.2014}: кроме данной абстракции остальные чисто инфраструртурные, эта же заточена на вполне конкретный вид сущностей - нужно перенести
    public interface IActivityPropertiesConverter<TEntity> :
        IDynamicEntityPropertiesConverter<TEntity, ActivityInstance, ActivityPropertyInstance>
        where TEntity : IEntity
    {
    }

    public interface IDynamicEntityPropertiesConverter<TEntity, TEntityInstance, TPropertyInstace>
        where TEntity : IEntity
        where TEntityInstance : IDynamicEntityInstance
        where TPropertyInstace : IDynamicEntityPropertyInstance
    {
        TEntity ConvertFromDynamicEntityInstance(TEntityInstance dynamicEntityInstance, ICollection<TPropertyInstace> propertyInstances);
        Tuple<TEntityInstance, ICollection<TPropertyInstace>> ConvertToDynamicEntityInstance(TEntity entity, ICollection<TPropertyInstace> propertyInstances, long? referencedEntityId);
    }
}