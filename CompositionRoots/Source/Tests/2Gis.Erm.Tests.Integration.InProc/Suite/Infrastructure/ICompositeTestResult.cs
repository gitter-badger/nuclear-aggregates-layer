using System.Collections.Generic;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public interface ICompositeTestResult
    {
        IEnumerable<ITestResult> Results { get; }
    }
}