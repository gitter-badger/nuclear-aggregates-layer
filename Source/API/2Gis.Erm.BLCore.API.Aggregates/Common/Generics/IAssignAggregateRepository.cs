using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    public interface IAssignAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<AssignIdentity>
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        int Assign(long entityId, long ownerCode);
    }
}