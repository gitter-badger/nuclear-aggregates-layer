using System;
using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class InProcTestRunner : ITestRunner
    {
        private readonly ITestSuite _testSuite;
        private readonly ITestFactory _testFactory;
        private readonly ITestStatusObserver _testStatusObserver;
        private readonly ICommonLog _logger;

        public InProcTestRunner(ITestSuite testSuite, ITestFactory testFactory, ITestStatusObserver testStatusObserver, ICommonLog logger)
        {
            _testSuite = testSuite;
            _testFactory = testFactory;
            _testStatusObserver = testStatusObserver;
            _logger = logger;
        }

        public TestResultsSet Run()
        {
            var succeeded  = new Dictionary<IIntegrationTest, ITestResult>();
            var failed  = new Dictionary<IIntegrationTest, ITestResult>();
            var ignored = new Dictionary<IIntegrationTest, ITestResult>();
            var unhandled = new Dictionary<IIntegrationTest, Exception>();
            var unresolved = new Dictionary<Type, Exception>();

            foreach (var integrationTestType in _testSuite.TestTypes)
            {
                _testStatusObserver.Started(integrationTestType);

                TestScope testScope;
                try
                {
                    testScope = _testFactory.Create(integrationTestType);
                }
                catch (Exception ex)
                {
                    _testStatusObserver.Unresolved(integrationTestType, ex);
                    unresolved.Add(integrationTestType, ex);
                    _logger.ErrorFormatEx(ex, "Can't create scope for test {0}", integrationTestType);
                    continue;
                }

                using (testScope)
                {
                    var testDescription = ExtractDescription(testScope.Test);

                    TransactionScope transactionScope = null;

                    try
                    {
                        _logger.InfoFormatEx("Starting test {0}", testDescription);

                        transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default);
                        var result = testScope.Test.Execute();

                        switch (result.Status)
                        {
                            case TestResultStatus.Failed:
                                _testStatusObserver.Asserted(integrationTestType, result);
                                failed.Add(testScope.Test, result);
                                _logger.InfoFormatEx("Failed {0} with result {1}", testDescription, result);
                                break;

                            case TestResultStatus.Succeeded:
                                _testStatusObserver.Succeeded(integrationTestType, result);
                                succeeded.Add(testScope.Test, result);
                                _logger.InfoFormatEx("Succeeded {0} with result {1}", testDescription, result);
                                break;

                            case TestResultStatus.Ignored:
                                _testStatusObserver.Ignored(integrationTestType, result);
                                ignored.Add(testScope.Test, result);
                                _logger.InfoFormatEx("Succeeded {0} with result {1}", testDescription, result);
                                break;

                            default:
                                throw new Exception("Unknown test result status");
                        }
                    }
                    catch (Exception ex)
                    {
                        _testStatusObserver.Unhandled(integrationTestType, ex);
                        unhandled.Add(testScope.Test, ex);
                        _logger.ErrorFormatEx(ex, "Exception in {0}", testDescription);
                    }
                    finally
                    {
                        if (transactionScope != null)
                        {
                            try
                            {
                                transactionScope.Dispose();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }

            return new TestResultsSet
                {
                    Succeeded = succeeded,
                    Failed = failed,
                    Ignored = ignored,
                    Unhandled = unhandled,
                    Unresolved = unresolved
                };
        }

        private static string ExtractDescription(IIntegrationTest test)
        {
            return test.GetType().FullName;
        }
    }
}
