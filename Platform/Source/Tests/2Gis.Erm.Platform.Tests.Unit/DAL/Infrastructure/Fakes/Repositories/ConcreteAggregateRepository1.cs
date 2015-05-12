using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;

using NuClear.Aggregates;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public interface IConcreteAggregateRepository1 : IAggregateRepository
    {
    }

    /// <summary>
    /// Stub репозиторий использующий зависимости (finder, сущностные репозитории и т.п.) - фактически alias закрывающий generic StubWithDependenciesAggregateRepository
    /// </summary>
    public sealed class ConcreteAggregateRepository1 : StubWithDependenciesAggregateRepository<ErmScopeEntity1, ErmScopeEntity2>, IConcreteAggregateRepository1
    {
        public ConcreteAggregateRepository1(StubFinder finder, StubEntityRepository<ErmScopeEntity1> entityRepositoryType1, StubEntityRepository<ErmScopeEntity2> entityRepositoryType2)
            : base(finder, entityRepositoryType1, entityRepositoryType2)
        {
        }
    }
}