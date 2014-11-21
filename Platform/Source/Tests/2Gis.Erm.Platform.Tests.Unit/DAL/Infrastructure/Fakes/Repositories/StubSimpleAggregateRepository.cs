using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public interface IStubSimpleAggregateRepository : IAggregateRepository
    {
    }

    public sealed class StubSimpleAggregateRepository : IStubSimpleAggregateRepository
    {
    }
}