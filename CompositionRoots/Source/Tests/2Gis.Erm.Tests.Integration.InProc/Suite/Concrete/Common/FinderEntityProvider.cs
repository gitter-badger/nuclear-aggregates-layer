using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public sealed class FinderAppropriateEntityProvider<TEntity> : IAppropriateEntityProvider<TEntity> 
        where TEntity : class, IEntity
    {
        private readonly IFinder _finder;

        public FinderAppropriateEntityProvider(IFinder finder)
        {
            _finder = finder;
        }

        public TEntity Get(FindSpecification<TEntity> spec)
        {
            return _finder.Find(spec).Top();
        }

        public IReadOnlyCollection<TEntity> Get(FindSpecification<TEntity> spec, int maxCount)
        {
            return _finder.Find(spec).Map(q => q.Take(maxCount)).Many();
        }
    }
}
