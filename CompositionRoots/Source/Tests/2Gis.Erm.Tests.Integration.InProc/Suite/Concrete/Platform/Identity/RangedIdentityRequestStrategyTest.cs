using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.IdentityService.Client.Interaction;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Identity
{
    public class IdentityServiceClientTest : IIntegrationTest
    {
        private const int NumOfTasks = 10;
        private const int RequestedIdentitiesCount = 1000;
        private const int RequestCount = 1000;

        private readonly IdentityServiceClient _identityServiceClient;
        private readonly ITracer _logger;

        public IdentityServiceClientTest(IdentityServiceClient identityServiceClient, ITracer logger)
        {
            _identityServiceClient = identityServiceClient;
            _logger = logger;
        }

        public ITestResult Execute()
        {
            var tasks = new Task<IEnumerable<long>>[NumOfTasks];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task<IEnumerable<long>>.Factory.StartNew(() => GetIdentities(_identityServiceClient));
            }

            var rangedSw = Stopwatch.StartNew();
            Task.WaitAll(tasks);
            rangedSw.Stop();

            var ids = new HashSet<long>();
            foreach (var task in tasks)
            {
                foreach (var id in task.Result)
                {
                    ids.Add(id);
                }
            }

            _logger.InfoFormat("Identity service client: {0, 10} ids/sec", tasks.Length * RequestedIdentitiesCount * RequestCount * 1000d / rangedSw.ElapsedMilliseconds);

            return ids.Count == (NumOfTasks * RequestedIdentitiesCount * RequestCount) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        private IEnumerable<long> GetIdentities(IIdentityServiceClient client)
        {
            var ids = new List<long>();
            for (int i = 0; i < RequestCount; i++)
            {
                ids.AddRange(client.GetIdentities(RequestedIdentitiesCount));
            }

            return ids;
        }
    }
}