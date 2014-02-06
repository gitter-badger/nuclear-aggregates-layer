using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Aggregates.Dynamic
{
    public interface IDynamicBusinessEntityService<TDynamicEntityInstance, in TTDynamicPropertyEntityInstance> : 
        ICreateAggregateRepository<TDynamicEntityInstance>,
        IUpdateAggregateRepository<TDynamicEntityInstance>,
        IDeleteAggregateRepository<TDynamicEntityInstance>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TTDynamicPropertyEntityInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        long Create(TDynamicEntityInstance entityInstance, IEnumerable<TTDynamicPropertyEntityInstance> propertyInstances);
        void Update(TDynamicEntityInstance entityInstance, IEnumerable<TTDynamicPropertyEntityInstance> propertyInstances);
    }
}