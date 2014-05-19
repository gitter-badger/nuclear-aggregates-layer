using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations
{
    public interface IUpdatePartableEntityAggregateService<TAggregateRoot, in TAggregatePart> : IAggregateSpecificOperation<TAggregateRoot, UpdateIdentity>
        where TAggregateRoot : class, IEntity, IEntityKey
        where TAggregatePart : class, IEntity, IEntityKey
    {
        void Update(TAggregatePart entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos);
    }
}