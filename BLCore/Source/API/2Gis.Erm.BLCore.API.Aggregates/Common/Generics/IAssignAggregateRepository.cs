using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IAssignAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<AssignIdentity>
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        int Assign(long entityId, long ownerCode);
    }
}