using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

        public TEntity Get(IFindSpecification<TEntity> findSpecification)
        {
            return _finder.Find(findSpecification).FirstOrDefault();
        }

        public TDto Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification)
        {
            return _finder.Find(selectSpecification, findSpecification).FirstOrDefault();
        }

        public IReadOnlyCollection<TEntity> Get(IFindSpecification<TEntity> findSpecification, int maxCount)
        {
            return _finder.Find(findSpecification).Take(maxCount).ToArray();
        }

        public IReadOnlyCollection<TDto> Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification, int maxCount)
        {
            return _finder.Find(selectSpecification, findSpecification).Take(maxCount).ToArray();
        }
    }
}
