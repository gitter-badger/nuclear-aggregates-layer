using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.NuClear.IdentityService.Client.Interaction;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Identity
{
    public class IdentityRequestStrategiesTest : IIntegrationTest
    {
        private const int NumOfTasks = 10;
        private const int RequestedIdentitiesCount = 100;
        private const int RequestCount = 1000;

        private readonly RangedIdentityRequestStrategy _rangedIdentityRequestStrategy;
        private readonly BufferedIdentityRequestStrategy _bufferedIdentityRequestStrategy;
        private readonly ITracer _logger;

        public IdentityRequestStrategiesTest(RangedIdentityRequestStrategy rangedIdentityRequestStrategy,
                                                 BufferedIdentityRequestStrategy bufferedIdentityRequestStrategy,
                                                 ITracer logger)
        {
            _rangedIdentityRequestStrategy = rangedIdentityRequestStrategy;
            _bufferedIdentityRequestStrategy = bufferedIdentityRequestStrategy;
            _logger = logger;
        }

        public ITestResult Execute()
        {
            var rangedStrategyTasks = new Task<IEnumerable<long>>[NumOfTasks / 2];
            for (int i = 0; i < rangedStrategyTasks.Length; i++)
            {
                rangedStrategyTasks[i] = Task<IEnumerable<long>>.Factory.StartNew(() => GetIdentities(_rangedIdentityRequestStrategy));
            }

            var rangedSw = Stopwatch.StartNew();
            Task.WaitAll(rangedStrategyTasks);
            rangedSw.Stop();


            var bufferedStrategyTasks = new Task<IEnumerable<long>>[NumOfTasks - (NumOfTasks / 2)];
            for (int i = 0; i < bufferedStrategyTasks.Length; i++)
            {
                bufferedStrategyTasks[i] = Task<IEnumerable<long>>.Factory.StartNew(() => GetIdentities(_bufferedIdentityRequestStrategy));
            }

            var bufferedSw = Stopwatch.StartNew();
            Task.WaitAll(bufferedStrategyTasks);
            bufferedSw.Stop();

            var ids = new HashSet<long>();
            foreach (var task in rangedStrategyTasks.Concat(bufferedStrategyTasks))
            {
                foreach (var id in task.Result)
                {
                    ids.Add(id);
                }
            }

            _logger.InfoFormat("Ranged strategy: {0, 10} ids/sec", rangedStrategyTasks.Length * RequestedIdentitiesCount * RequestCount * 1000d / rangedSw.ElapsedMilliseconds);
            _logger.InfoFormat("Buffered strategy: {0, 10} ids/sec", bufferedStrategyTasks.Length * RequestedIdentitiesCount * RequestCount * 1000d / bufferedSw.ElapsedMilliseconds);

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