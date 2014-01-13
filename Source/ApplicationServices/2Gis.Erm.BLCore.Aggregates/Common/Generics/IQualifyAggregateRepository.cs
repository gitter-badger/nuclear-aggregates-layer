using System;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IQualifyAggregateRepository<TEntity> : IUnknownAggregateSpecificOperation<QualifyIdentity>
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        int Qualify(long entityId, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
    }
}