using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.Core.Identities;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Identity
{
    public class BufferedIdentityRequestStrategyTest : IIntegrationTest
    {
        private readonly BufferedIdentityRequestStrategy _bufferedIdentityRequestStrategy;

        private const int NumOfTasks = 10;
        private const int RequestedIdentitiesCount = 10;
        private const int RequestCount = 10;

        public BufferedIdentityRequestStrategyTest(BufferedIdentityRequestStrategy bufferedIdentityRequestStrategy)
        {
            _bufferedIdentityRequestStrategy = bufferedIdentityRequestStrategy;
        }

        public ITestResult Execute()
        {
            var set = new HashSet<long>();
            var tasks = new Task<IEnumerable<long>>[NumOfTasks];

            for (int i = 0; i < NumOfTasks; i++)
            {
                tasks[i] = Task<IEnumerable<long>>.Factory.StartNew(GetIdentities);
            }

            Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                foreach (var id in task.Result)
                {
                    set.Add(id);
                }
            }

            return set.Count == (NumOfTasks * RequestedIdentitiesCount * RequestCount) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        private IEnumerable<long> GetIdentities()
        {
            var ids = new List<long>();
            for (int i = 0; i < RequestCount; i++)
            {
                ids.AddRange(_bufferedIdentityRequestStrategy.Request(RequestedIdentitiesCount));
            }

            return ids;
        }
    }
}