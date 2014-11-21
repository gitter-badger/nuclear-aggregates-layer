using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.BDD
{
    public sealed class BDDTestRunConfig<TTestArgs, TTestResult>
        where TTestArgs : class, IBDDTestArgs, new()
        where TTestResult : class, ITestResult
    {
        public TTestArgs Args { get; set; }
        public Action<TTestArgs, TTestResult> ThenValidator { get; set; }
    }
}