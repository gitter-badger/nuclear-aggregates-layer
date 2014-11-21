using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public interface ITestSuite
    {
        IEnumerable<Type> TestTypes { get; }
    }
}