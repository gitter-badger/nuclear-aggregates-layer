using System;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IChangeAggregateTerritoryRepository<TEntity> : IUnknownAggregateSpecificService<ChangeTerritoryIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        int ChangeTerritory(long entityId, long territoryId);
    }
}