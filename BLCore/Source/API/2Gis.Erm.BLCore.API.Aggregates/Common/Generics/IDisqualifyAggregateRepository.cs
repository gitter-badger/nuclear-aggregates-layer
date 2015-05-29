using System;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IDisqualifyAggregateRepository<TEntity> : IUnknownAggregateSpecificService<DisqualifyIdentity>
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        int Disqualify(long entityId, long currentUserCode, long reserveCode, bool bypassValidation, DateTime disqualifyDate);
    }
}