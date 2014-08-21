using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IUpdateAggregateRepository<in TEntity> : IUnknownAggregateSpecificOperation<UpdateIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        int Update(TEntity entity);
    }
}