using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    /// <summary>
    /// Stub для агрегирующего репозитория сущностей TEntity1_TEntity2.
    /// Требует для работы зависимости от Finder и двух сущностных репозиториев.
    /// </summary>
    /// <typeparam name="TEntity1">Тип сущности используемой 1ым сущностным репозиторием</typeparam>
    /// <typeparam name="TEntity2">Тип сущности используемой 2ым сущностным репозиторием</typeparam>
    public abstract class StubWithDependenciesAggregateRepository<TEntity1, TEntity2>
        where TEntity1 : class, IEntity
        where TEntity2 : class, IEntity
    {
        private readonly StubFinder _finder;
        private readonly StubEntityRepository<TEntity1> _entityRepositoryType1;
        private readonly StubEntityRepository<TEntity2> _entityRepositoryType2;

        protected StubWithDependenciesAggregateRepository(StubFinder finder,
                                                          StubEntityRepository<TEntity1> entityRepositoryType1,
                                                          StubEntityRepository<TEntity2> entityRepositoryType2)
        {
            _finder = finder;
            _entityRepositoryType1 = entityRepositoryType1;
            _entityRepositoryType2 = entityRepositoryType2;
        }

        public StubEntityRepository<TEntity1> EntityRepositoryType1
        {
            get
            {
                return _entityRepositoryType1;
            }
        }

        public StubEntityRepository<TEntity2> EntityRepositoryType2
        {
            get
            {
                return _entityRepositoryType2;
            }
        }

        public StubFinder Finder
        {
            get
            {
                return _finder;
            }
        }
    }
}
