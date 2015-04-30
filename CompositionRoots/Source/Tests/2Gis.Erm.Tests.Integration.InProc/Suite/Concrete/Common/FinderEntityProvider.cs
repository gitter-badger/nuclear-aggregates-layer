using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
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
            return _finder.FindOne(findSpecification);
        }

        public TDto Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification)
        {
            return _finder.Find(selectSpecification, findSpecification).FirstOrDefault();
        }

        public IReadOnlyCollection<T> Get<T>(IFindSpecification<T> findSpecification, int maxCount)
            where T : class, TEntity, IEntityKey
        {
            var ids = _finder.Find(Specs.Select.Id<T>(), findSpecification).Take(maxCount).ToArray();
            return _finder.FindMany(Specs.Find.ByIds<T>(ids)).ToArray();
        }

        public IReadOnlyCollection<TDto> Get<TDto>(IFindSpecification<TEntity> findSpecification, ISelectSpecification<TEntity, TDto> selectSpecification, int maxCount)
        {
            return _finder.Find(selectSpecification, findSpecification).Take(maxCount).ToArray();
        }
    }
}
