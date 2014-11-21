using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TestSuite : ITestSuite
    {
        private readonly IEnumerable<Type> _testTypes;

        public TestSuite(IEnumerable<Type> testTypes)
        {
            _testTypes = testTypes;
        }

        public IEnumerable<Type> TestTypes
        {
            get { return _testTypes; }
        }
    }
}