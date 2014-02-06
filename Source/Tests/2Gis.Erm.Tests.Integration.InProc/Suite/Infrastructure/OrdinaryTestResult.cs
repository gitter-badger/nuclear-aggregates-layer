using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class OrdinaryTestResult : ITestResult
    {
        public static class As
        {
            public static OrdinaryTestResult Succeeded
            {
                get
                {
                    return new OrdinaryTestResult().AsSucceeded();
                }
            }

            public static OrdinaryTestResult Failed
            {
                get
                {
                    return new OrdinaryTestResult().AsFailed();
                }
            }

            public static OrdinaryTestResult Ignored
            {
                get
                {
                    return new OrdinaryTestResult().AsIgnored();
                }
            }

            public static OrdinaryTestResult Asserted(Exception assertionException)
            {
                return new OrdinaryTestResult().AsFailed().AsAsserted(assertionException);
            }

            public static OrdinaryTestResult Unhandled(Exception unhandledException)
            {
                return new OrdinaryTestResult().AsFailed().AsUnhandled(unhandledException);
            }

            public static OrdinaryTestResult NotExecuted
            {
                get { return Ignored.WithReport("Test has not been executed"); }
            }

        }

        private OrdinaryTestResult()
        {
        }

        public TestResultStatus Status { get; private set; }
        public string Report { get; private set; }
        public Exception Asserted { get; private set; }
        public Exception Unhandled { get; private set; }

        public OrdinaryTestResult AsSucceeded()
        {
            Status = TestResultStatus.Succeeded;
            return this;
        }

        public OrdinaryTestResult AsFailed()
        {
            Status = TestResultStatus.Failed;
            return this;
        }

        public OrdinaryTestResult AsIgnored()
        {
            Status = TestResultStatus.Ignored;
            return this;
        }

        public OrdinaryTestResult AsAsserted(Exception assertionException)
        {
            Status = TestResultStatus.Failed;
            Asserted = assertionException;
            return this;
        }

        public OrdinaryTestResult AsUnhandled(Exception unhandledException)
        {
            Status = TestResultStatus.Failed;
            Unhandled = unhandledException;
            return this;
        }
        
        public OrdinaryTestResult WithReport(string report)
        {
            Report = report;
            return this;
        }
        
        public OrdinaryTestResult WithReport(string reportTemplate, params object[] values)
        {
            Report = string.Format(reportTemplate, values);
            return this;
        }

        public override string ToString()
        {
            return Status.ToString();
        }
    }
}