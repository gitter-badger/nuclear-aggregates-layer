using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing.Analysis
{
    public sealed partial class PerformedOperationsConsumerJob : TaskServiceJobBase, IInterruptableJob
    {
        private const int DefaultBatchSize = 1000;
        
        private readonly IMessageFlowRegistry _messageFlowRegistry;
        private readonly IServiceBusMessageReceiverSettings _serviceBusMessageReceiverSettings;
        private readonly CancellationTokenSource _consumersCancellationTokenSource = new CancellationTokenSource();

        public PerformedOperationsConsumerJob(
            IMessageFlowRegistry messageFlowRegistry,
            IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings,
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService, 
            ICommonLog logger) 
            : base(signInService, userImpersonationService, logger)
        {
            _messageFlowRegistry = messageFlowRegistry;
            _serviceBusMessageReceiverSettings = serviceBusMessageReceiverSettings;
        }

        public int? BatchSize { get; set; }

        public void Interrupt()
        {
            Logger.InfoEx("Consuming performed operations. Interrupt called for job, consuming performed operations is stopping");
            _consumersCancellationTokenSource.Cancel();
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Logger.InfoEx("Consuming performed operations. Processing started");

            var workers = ResolveWorkersForPerformedOperationsSources();
            Task.WaitAll(workers);

            Logger.InfoEx("Consuming performed operations. Processing stopped");
        }

        private Task[] ResolveWorkersForPerformedOperationsSources()
        {
            return _messageFlowRegistry.Flows
                        .Where(PrimaryProcessing.IsPerformedOperationsSourceFlow)
                        .Select(performedOperationsSourceFlow => typeof(PerformedOperationsFlowConsumer<>).MakeGenericType(performedOperationsSourceFlow.GetType()))
                        .Select(flowConsumerType =>
                            flowConsumerType.New<IPerformedOperationsConsumer>(_serviceBusMessageReceiverSettings, BatchSize ?? DefaultBatchSize, _consumersCancellationTokenSource.Token, Logger))
                        .Select(c => c.Consume())
                        .ToArray();
        }
    }
}
