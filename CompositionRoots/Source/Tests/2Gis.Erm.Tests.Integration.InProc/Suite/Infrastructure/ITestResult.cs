using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public interface ITestResult
    {
        string Report { get; }
        Exception Asserted { get; }
        Exception Unhandled { get; }
        TestResultStatus Status { get; }
    }
}