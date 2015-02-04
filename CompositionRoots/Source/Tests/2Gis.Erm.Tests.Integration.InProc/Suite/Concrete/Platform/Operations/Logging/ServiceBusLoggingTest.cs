using System.Diagnostics;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Logging
{
    public sealed class ServiceBusLoggingTest : IIntegrationTest
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ServiceBusLoggingTest(
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public ITestResult Execute()
        {
            return Result
                .When(EmulateOperationsExecution)
                .Then(result => result.Status == TestResultStatus.Succeeded);
        }

        private void EmulateOperationsExecution()
        {
            var profiler = Stopwatch.StartNew();
            const int TargetExecutionCount = 1;

            try
            {
                for (int i = 0; i < TargetExecutionCount; i++)
                {
                    RootWithNested();
                }
                
            }
            finally
            {
                profiler.Stop();
                decimal elapsedSeconds = profiler.ElapsedMilliseconds / 1000M;
                _logger.InfoFormat("Execution count: {0}, it takes {1} sec, result processing rate exec/sec: {2}", 
                    TargetExecutionCount,
                    elapsedSeconds,
                    TargetExecutionCount / elapsedSeconds);
            }
        }

        private void RootWithNested()
        {
            using (var rootScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                using (var nestedScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, OrderPosition>())
                {
                    for (long i = 0; i < 1; i++)
                    {
                        rootScope.Updated<OrderPosition>(i);
                    }

                    nestedScope.Complete();
                }

                for (long i = 0; i < 1; i++)
                {
                    rootScope.Updated<Order>(i);
                }

                rootScope.Complete();
            }
        }
    }
}
