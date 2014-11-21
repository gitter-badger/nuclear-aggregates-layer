using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public class StreamResponseAsserter : IResponseAsserter<StreamResponse>
    {
        public OrdinaryTestResult Assert(StreamResponse result)
        {
            return Result.When(result)
                         .Then(r => r.Stream.Should().NotBeNull());
        }
    }
}