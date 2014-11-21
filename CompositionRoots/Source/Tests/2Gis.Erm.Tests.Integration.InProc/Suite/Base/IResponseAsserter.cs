using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public interface IResponseAsserter<in T>
    {
        OrdinaryTestResult Assert(T result);
    }
}