using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public class EmptyResponseAsserter : IResponseAsserter<Response>
    {
        public OrdinaryTestResult Assert(Response result)
        {
            return OrdinaryTestResult.As.Succeeded;
        }
    }
}