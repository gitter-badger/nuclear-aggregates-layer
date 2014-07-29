using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    public interface ICreateAggregateRepository<in TEntity> : IUnknownAggregateSpecificOperation<CreateIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        int Create(TEntity entity);
    }
}