using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public interface IAppropriateEntityProvider<TEntity> where TEntity : class, IEntity
    {
        TEntity Get(IFindSpecification<TEntity> spec);
        IReadOnlyCollection<TEntity> Get(IFindSpecification<TEntity> spec, int maxCount);
    }
}