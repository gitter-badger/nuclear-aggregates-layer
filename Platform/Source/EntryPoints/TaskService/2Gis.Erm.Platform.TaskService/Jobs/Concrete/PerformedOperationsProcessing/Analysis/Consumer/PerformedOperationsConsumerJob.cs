using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Security;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer
{
    [DisallowConcurrentExecution]
    public sealed class PerformedOperationsConsumerJob : TaskServiceJobBase, IInterruptableJob
    {
        private const int DefaultBatchSize = 1000;
        private const PerformedOperationsTransport DefaultOperationsTransport = PerformedOperationsTransport.ServiceBus;
        
        private readonly IMessageFlowRegistry _messageFlowRegistry;
        private readonly IPerformedOperationsConsumerFactory _performedOperationsConsumerFactory;
        private readonly CancellationTokenSource _consumersCancellationTokenSource = new CancellationTokenSource();

        public PerformedOperationsConsumerJob(
            IMessageFlowRegistry messageFlowRegistry,
            IPerformedOperationsConsumerFactory performedOperationsConsumerFactory,
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService, 
            ICommonLog logger) 
            : base(signInService, userImpersonationService, logger)
        {
            _messageFlowRegistry = messageFlowRegistry;
            _performedOperationsConsumerFactory = performedOperationsConsumerFactory;
        }

        public int? BatchSize { get; set; }
        public PerformedOperationsTransport? OperationsTransport { get; set; }

        public void Interrupt()
        {
            Logger.Info("Consuming performed operations. Interrupt called for job, consuming performed operations is stopping");
            _consumersCancellationTokenSource.Cancel();
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Logger.Info("Consuming performed operations. Processing started");

            var workers = ResolveWorkersForPerformedOperationsSources();
            Task.WaitAll(workers);

            Logger.Info("Consuming performed operations. Processing stopped");
        }

        private Task[] ResolveWorkersForPerformedOperationsSources()
        {
            return _messageFlowRegistry.Flows
                        .Where(PrimaryProcessing.IsPerformedOperationsSourceFlow)
                        .Select(performedOperationsSourceFlow => 
                            _performedOperationsConsumerFactory.Create(
                                performedOperationsSourceFlow.GetType(),
                                OperationsTransport ?? DefaultOperationsTransport,
                                BatchSize ?? DefaultBatchSize,
                                _consumersCancellationTokenSource.Token))
                        .Select(c => c.Consume())
                        .ToArray();
        }
    }
}
