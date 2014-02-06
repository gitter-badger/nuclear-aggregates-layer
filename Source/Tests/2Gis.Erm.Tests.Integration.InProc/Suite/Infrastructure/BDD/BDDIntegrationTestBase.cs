using System;
using System.Collections.Generic;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.BDD
{
    public abstract class BDDIntegrationTestBase<TTestArgs, TTestResult> : IIntegrationTest
        where TTestArgs : class, IBDDTestArgs, new()
        where TTestResult : class, ITestResult
    {
        public ITestResult Execute()
        {
            var compositeResult = new CompositeTestResult();

            foreach (var testRun in Given())
            {
                try
                {
                    var result = When(testRun.Args);
                    if (result.Status != TestResultStatus.Succeeded)
                    {
                        compositeResult.Add(
                            OrdinaryTestResult.As.Failed
                                .WithReport("Given {0}. When failed with {1}. Then not evaluated", testRun.Args.GivenDescription, result.Report));
                        continue;
                    }

                    try
                    {
                        if (testRun.ThenValidator != null)
                        {
                            testRun.ThenValidator(testRun.Args, result);
                        }
                        else
                        {
                            Then(testRun.Args, result);
                        }

                        compositeResult.Add(OrdinaryTestResult.As.Succeeded);
                    }
                    catch (Exception ex)
                    {
                        compositeResult.Add(
                            OrdinaryTestResult.As
                                .Asserted(ex)
                                .WithReport("Given {0}. When {1}. Then assserted with {2}", testRun.Args.GivenDescription, "EMPTY_DESCIPTION", ex.Message));
                    }
                }
                catch (Exception ex)
                {
                    compositeResult.Add(OrdinaryTestResult.As
                                .Unhandled(ex)
                                .WithReport("Given {0}. When unhandled failed with {1}. Then not evaluated", testRun.Args.GivenDescription, ex.Message));
                }
            }

            return compositeResult;
        }

        protected virtual IEnumerable<BDDTestRunConfig<TTestArgs, TTestResult>> Given()
        {
            return new[] { new BDDTestRunConfig<TTestArgs, TTestResult> { Args = new TTestArgs() } };
        }

        protected abstract TTestResult When(TTestArgs arg);

        protected virtual void Then(TTestArgs args, TTestResult result)
        {
            result.Status.Should().Be(TestResultStatus.Succeeded);
        }
    }
}