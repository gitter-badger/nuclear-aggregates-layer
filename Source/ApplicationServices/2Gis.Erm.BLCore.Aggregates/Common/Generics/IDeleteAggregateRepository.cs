using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IDeleteAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<DeleteIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        int Delete(long entityId);
    }
}