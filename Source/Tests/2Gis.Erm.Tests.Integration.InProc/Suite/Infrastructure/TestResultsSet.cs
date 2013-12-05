using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TestResultsSet
    {
        public IReadOnlyDictionary<IIntegrationTest, ITestResult> Succeeded { get; set; }
        public IReadOnlyDictionary<IIntegrationTest, ITestResult> Failed { get; set; }
        public IReadOnlyDictionary<IIntegrationTest, Exception> Unhandled { get; set; }
        public IReadOnlyDictionary<Type, Exception> Unresolved { get; set; }

        public int TotalCount 
        {
            get { return Succeeded.Count + Failed.Count + Unhandled.Count + Unresolved.Count; }
        }

        public bool Passed
        {
            get { return !(Failed.Any() || Unhandled.Any() || Unresolved.Any()); }
        }
    }
}