using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class NullTestStatusObserver : ITestStatusObserver
    {
        public void Started(Type testType)
        {
        }

        public void Unresolved(Type testType, Exception exception)
        {
        }

        public void Unhandled(Type testType, Exception exception)
        {
        }

        public void Asserted(Type testType, ITestResult testResult)
        {
        }

        public void Succeeded(Type testType, ITestResult testResult)
        {
        }

        public void Ignored(Type testType, ITestResult testResult)
        {
        }
    }
}