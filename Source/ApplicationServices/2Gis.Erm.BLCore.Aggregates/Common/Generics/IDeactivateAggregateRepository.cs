using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IDeactivateAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<DeactivateIdentity>
        where TEntity : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        int Deactivate(long entityId);
    }
}