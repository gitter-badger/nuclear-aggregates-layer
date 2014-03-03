using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations
{
    public interface ICreatePartableEntityAggregateService<TAggregateRoot, in TAggregatePart> : IAggregateSpecificOperation<TAggregateRoot, CreateIdentity>
        where TAggregateRoot : class, IEntity, IEntityKey
        where TAggregatePart : class, IEntity, IEntityKey
    {
        long Create(TAggregatePart entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos);
    }
}