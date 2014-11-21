using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    public interface IUpdateDynamicAggregateRepository<in TDynamicEntityInstance, in TDynamicEntityPropertyInstance> :
        IUnknownAggregateSpecificOperation<UpdateIdentity>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TDynamicEntityPropertyInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        void Update(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicEntityPropertyInstance> propertyInstances);
    }
}