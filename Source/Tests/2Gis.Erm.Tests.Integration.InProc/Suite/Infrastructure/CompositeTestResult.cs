using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class CompositeTestResult : ITestResult, ICompositeTestResult
    {
        private readonly ICollection<ITestResult> _results = new List<ITestResult>();

        public CompositeTestResult Add(ITestResult result)
        {
            _results.Add(result);
            return this;
        }

        public string Report
        {
            get
            {
                return _results.Aggregate(new StringBuilder(), (builder, result) => builder.AppendLine(result.Report)).ToString();
            }
        }

        public Exception Asserted
        {
            get 
            { 
                return new AggregateException(
                    _results
                        .Where(result => result.Status != TestResultStatus.Succeeded && result.Asserted != null)
                        .Select(result => result.Asserted));
            }
        }

        public Exception Unhandled
        {
            get
            {
                return new AggregateException(
                    _results
                        .Where(result => result.Status != TestResultStatus.Succeeded && result.Unhandled != null)
                        .Select(result => result.Unhandled));
            }
        }

        public TestResultStatus Status
        {
            get 
            { 
                return _results.All(result => result.Status == TestResultStatus.Succeeded) 
                    ? TestResultStatus.Succeeded
                    : TestResultStatus.Failed;
            }
        }

        public IEnumerable<ITestResult> Results
        {
            get { return _results; }
        }
    }
}