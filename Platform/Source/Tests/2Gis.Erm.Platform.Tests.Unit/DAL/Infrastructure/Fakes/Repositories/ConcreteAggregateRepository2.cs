using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;

using NuClear.Aggregates;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public interface IConcreteAggregateRepository2 : IAggregateRepository
    {
    }

    /// <summary>
    /// Stub репозиторий использующий зависимости (finder, сущностные репозитории и т.п.) - фактически alias закрывающий generic StubWithDependenciesAggregateRepository
    /// </summary>
    public sealed class ConcreteAggregateRepository2 : StubWithDependenciesAggregateRepository<ErmScopeEntity2, ErmScopeEntity1>, IConcreteAggregateRepository2
    {
        public ConcreteAggregateRepository2(StubFinder finder, StubEntityRepository<ErmScopeEntity2> entityRepositoryType1, StubEntityRepository<ErmScopeEntity1> entityRepositoryType2)
            : base(finder, entityRepositoryType1, entityRepositoryType2)
        {
        }
    }
}