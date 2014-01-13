using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IDisqualifyAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<DisqualifyIdentity>
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        int Disqualify(long entityId, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate);
    }
}