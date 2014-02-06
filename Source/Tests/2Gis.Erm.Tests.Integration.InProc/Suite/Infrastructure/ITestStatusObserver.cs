using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public interface ITestStatusObserver
    {
        void Started(Type testType);
        void Unresolved(Type testType, Exception exception);
        void Unhandled(Type testType, Exception exception);
        void Asserted(Type testType, ITestResult testResult);
        void Succeeded(Type testType, ITestResult testResult);
        void Ignored(Type testType, ITestResult testResult);
    }
}