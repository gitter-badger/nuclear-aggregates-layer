using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public interface ITestFactory
    {
        TestScope Create(Type testType);
    }
}