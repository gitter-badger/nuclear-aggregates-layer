using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.DAL.Specifications;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
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

        public TEntity Get(FindSpecification<TEntity> findSpecification)
        {
            return _finder.Find(findSpecification).One();
        }

        public TDto Get<TDto>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TDto> selectSpecification)
        {
            return _finder.Find(findSpecification).Map(q => q.Select(selectSpecification)).Top();
        }

        public IReadOnlyCollection<T> Get<T>(FindSpecification<T> findSpecification, int maxCount)
            where T : class, TEntity, IEntityKey
        {
            var ids = _finder.Find(findSpecification).Map(q => q.Select(Specs.Select.Id<T>()).Take(maxCount)).Many();
            return _finder.Find(Specs.Find.ByIds<T>(ids)).Many();
        }

        public IReadOnlyCollection<TDto> Get<TDto>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TDto> selectSpecification, int maxCount)
        {
            return _finder.Find(findSpecification).Map(q => q.Select(selectSpecification).Take(maxCount)).Many();
        }
    }
}
