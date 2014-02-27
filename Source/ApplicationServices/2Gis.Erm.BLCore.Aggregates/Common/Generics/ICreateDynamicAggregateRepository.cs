using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface ICreateDynamicAggregateRepository<in TDynamicEntityInstance, in TDynamicEntityPropertyInstance> : IUnknownAggregateSpecificOperation<CreateIdentity>
        where TDynamicEntityInstance : class, IEntity, IEntityKey, IDynamicEntityInstance
        where TDynamicEntityPropertyInstance : class, IEntity, INonActivityDynamicEntityPropertyInstance
    {
        long Create(TDynamicEntityInstance entityInstance, IEnumerable<TDynamicEntityPropertyInstance> propertyInstances);
    }
}