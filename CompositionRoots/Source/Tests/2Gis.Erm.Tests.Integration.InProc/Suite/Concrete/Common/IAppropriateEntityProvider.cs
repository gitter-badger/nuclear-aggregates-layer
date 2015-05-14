using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public interface IAppropriateEntityProvider<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(FindSpecification<TEntity> spec);
        IReadOnlyCollection<TEntity> Get(FindSpecification<TEntity> spec, int maxCount);
    }
}