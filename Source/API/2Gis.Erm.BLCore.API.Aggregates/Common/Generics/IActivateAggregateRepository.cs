using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IActivateAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<ActivateIdentity>
        where TEntity : class, IEntity, IEntityKey, IDeactivatableEntity
    {
        int Activate(long entityId);
    }
}