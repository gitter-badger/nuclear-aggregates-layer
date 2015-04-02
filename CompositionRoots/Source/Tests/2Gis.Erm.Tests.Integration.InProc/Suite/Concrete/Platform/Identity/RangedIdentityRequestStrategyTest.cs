using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.IdentityService.Client.Interaction;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Identity
{
    public class RangedIdentityRequestStrategyTest : IIntegrationTest
    {
        private const int NumOfTasks = 10;
        private const int RequestedIdentitiesCount = 1000;
        private const int RequestCount = 1000;

        private readonly RangedIdentityRequestStrategy _rangedIdentityRequestStrategy;
        private readonly ITracer _logger;

        public RangedIdentityRequestStrategyTest(RangedIdentityRequestStrategy rangedIdentityRequestStrategy,
                                                 ITracer logger)
        {
            _rangedIdentityRequestStrategy = rangedIdentityRequestStrategy;
            _logger = logger;
        }

        public ITestResult Execute()
        {
            var rangedStrategyTasks = new Task<IEnumerable<long>>[NumOfTasks];
            for (int i = 0; i < rangedStrategyTasks.Length; i++)
            {
                rangedStrategyTasks[i] = Task<IEnumerable<long>>.Factory.StartNew(() => GetIdentities(_rangedIdentityRequestStrategy));
            }

            var rangedSw = Stopwatch.StartNew();
            Task.WaitAll(rangedStrategyTasks);
            rangedSw.Stop();

            var ids = new HashSet<long>();
            foreach (var task in rangedStrategyTasks)
            {
                foreach (var id in task.Result)
                {
                    ids.Add(id);
                }
            }

            _logger.InfoFormat("Ranged strategy: {0, 10} ids/sec", rangedStrategyTasks.Length * RequestedIdentitiesCount * RequestCount * 1000d / rangedSw.ElapsedMilliseconds);
            
            return ids.Count == (NumOfTasks * RequestedIdentitiesCount * RequestCount) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        private IEnumerable<long> GetIdentities(IIdentityRequestStrategy strategy)
        {
            var ids = new List<long>();
            for (int i = 0; i < RequestCount; i++)
            {
                ids.AddRange(strategy.Request(RequestedIdentitiesCount));
            }

            return ids;
        }
    }
}