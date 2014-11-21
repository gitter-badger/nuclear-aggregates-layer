using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public interface IConcreteAggregateRepository2 : IAggregateRepository
    {
    }

    /// <summary>
    /// Stub ����������� ������������ ����������� (finder, ���������� ����������� � �.�.) - ���������� alias ����������� generic StubWithDependenciesAggregateRepository
    /// </summary>
    public sealed class ConcreteAggregateRepository2 : StubWithDependenciesAggregateRepository<ErmScopeEntity2, ErmScopeEntity1>, IConcreteAggregateRepository2
    {
        public ConcreteAggregateRepository2(StubFinder finder, StubEntityRepository<ErmScopeEntity2> entityRepositoryType1, StubEntityRepository<ErmScopeEntity1> entityRepositoryType2)
            : base(finder, entityRepositoryType1, entityRepositoryType2)
        {
        }
    }
}