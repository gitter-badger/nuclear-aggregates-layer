using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    public sealed class PrimaryProcessingSimulationSQLTransportLoadTest : IIntegrationTest
    {
        private const int ProcessingBatchSize = 1500;
        private const int FailedProcessingsCount = ProcessingBatchSize / 100;

        private readonly IOperationsPrimaryProcessingAbandonAggregateService _operationsPrimaryProcessingAbandonAggregateService;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;

        private readonly IDictionary<string, ProcessingStatistics> _processingStatisticsStore = new Dictionary<string, ProcessingStatistics>();

        private readonly IReadOnlyCollection<IMessageFlow> _targetMessageFlows = new IMessageFlow[]
                                                                    {
                                                                        PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance,
                                                                        PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance,
                                                                        PrimaryProcessHotClientPerformedOperationsFlow.Instance
                                                                    };

        private readonly IPerformedOperationsProcessingReadModel _performedOperationsProcessingReadModel;
        private readonly IRepository<PerformedOperationPrimaryProcessing> _primaryProcessingsRepository;
        private readonly ITracer _tracer;

        public PrimaryProcessingSimulationSQLTransportLoadTest(
            IPerformedOperationsProcessingReadModel performedOperationsProcessingReadModel,
            IRepository<PerformedOperationPrimaryProcessing> primaryProcessingsRepository,
            IOperationsPrimaryProcessingAbandonAggregateService operationsPrimaryProcessingAbandonAggregateService,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService,
            ITracer tracer)
        {
            _performedOperationsProcessingReadModel = performedOperationsProcessingReadModel;
            _primaryProcessingsRepository = primaryProcessingsRepository;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
            _tracer = tracer;
        }

        public ITestResult Execute()
        {
            const int TestSequenceLength = 100000;
            int effectiveQueueLength = TestSequenceLength * _targetMessageFlows.Count;

            _tracer.InfoFormat("Test started. Test sequence length:{0}. Effective sequence length: {1}", TestSequenceLength, effectiveQueueLength);

            var primaryProcessings = CreatePrimaryProcessings(TestSequenceLength);
            var stopwatch = Stopwatch.StartNew();
            PushToQueue(primaryProcessings);
            stopwatch.Stop();
            AttachStatistics("Enqueue", stopwatch.Elapsed.TotalSeconds, effectiveQueueLength);

            int counter = 0;
            while (_targetMessageFlows.Any(TryGetAndProcessNextBatch))
            {
                ++counter;
            }

            double totalTimeSec = 0;

            var reportBuilder = new StringBuilder();
            reportBuilder.AppendFormat("Test finished. Sequence effective length: {0}. Cycles passed: {1}{2}",
                                       effectiveQueueLength,
                                       counter,
                                       Environment.NewLine);

            _processingStatisticsStore.Aggregate(
                reportBuilder, 
                (builder, pair) => 
                    {  
                        var statistics = pair.Value;
                        totalTimeSec += statistics.TotalTimeSec;

                        return builder.AppendFormat("{0}. Avg time {1:N4} sec. Cycles: {2}. Rate: {3:N4} entry/sec.{4}",
                                                          pair.Key,
                                                          statistics.TotalTimeSec / statistics.ProcessingCyclesCount,
                                                          statistics.ProcessingCyclesCount,
                                                          statistics.ProcessedElementsCount / statistics.TotalTimeSec,
                                                          Environment.NewLine);
                    })
                .AppendFormat("Total avg time {0} sec. Rate: {1:N2} entry/sec{2}", totalTimeSec, effectiveQueueLength / totalTimeSec, Environment.NewLine);

            _tracer.Info("Test report: " + reportBuilder);
            return OrdinaryTestResult.As.Succeeded;
        }

        private bool TryGetAndProcessNextBatch(IMessageFlow messageFlow)
        {
            var stopwatch = Stopwatch.StartNew();
            var flowProcessingState = _performedOperationsProcessingReadModel.GetPrimaryProcessingFlowState(messageFlow);
            if (flowProcessingState == null || flowProcessingState.ProcessingTargetsCount == 0)
            {
                stopwatch.Stop();
                return false;
            }

            DateTime oldestOperationBoundaryDate = flowProcessingState.OldestProcessingTargetCreatedDate.Subtract(TimeSpan.FromHours(-2));
            var targetMessages = _performedOperationsProcessingReadModel.GetOperationsForPrimaryProcessing(messageFlow, oldestOperationBoundaryDate, ProcessingBatchSize);
            stopwatch.Stop();
            AttachStatistics("Acquire", stopwatch.Elapsed.TotalSeconds, targetMessages.Count);

            var succeededProcessings = new List<PerformedOperationPrimaryProcessing>();
            var failedProcessings = new List<PerformedOperationPrimaryProcessing>();

            for (int i = 0; i < targetMessages.Count; i++)
            {
                var currentMessageProcessing = targetMessages[i].TargetUseCase;
                if (targetMessages.Count == ProcessingBatchSize
                    && (i + FailedProcessingsCount) > targetMessages.Count)
                {
                    failedProcessings.Add(currentMessageProcessing);
                    continue;
                }

                succeededProcessings.Add(currentMessageProcessing);
            }

            stopwatch.Restart();
            _operationsPrimaryProcessingAbandonAggregateService.Abandon(failedProcessings);
            stopwatch.Stop();
            AttachStatistics("Update", stopwatch.Elapsed.TotalSeconds, failedProcessings.Count);

            stopwatch.Restart();
            _operationsPrimaryProcessingCompleteAggregateService.CompleteProcessing(succeededProcessings);
            stopwatch.Stop();
            AttachStatistics("Dequeue", stopwatch.Elapsed.TotalSeconds, succeededProcessings.Count);

            return true;
        }

        private void AttachStatistics(string operationKey, double elapsedSec, int processedElementsCount)
        {
            ProcessingStatistics statistics;
            if (!_processingStatisticsStore.TryGetValue(operationKey, out statistics))
            {
                statistics = new ProcessingStatistics();
                _processingStatisticsStore.Add(operationKey, statistics);
            }

            ++statistics.ProcessingCyclesCount;
            statistics.TotalTimeSec += elapsedSec;
            statistics.ProcessedElementsCount += processedElementsCount;

            _tracer.InfoFormat("{0}. Finished and it takes {1} sec", operationKey, elapsedSec);
        }

        private IReadOnlyList<PerformedOperationPrimaryProcessing> CreatePrimaryProcessings(int count)
        {
            var store = new List<PerformedOperationPrimaryProcessing>(count);
            for (int i = 0; i < count; i++)
            {
                Guid useCaseId = Guid.NewGuid();
                DateTime currentTime = DateTime.UtcNow;

                store.AddRange(_targetMessageFlows.Select(flow => new PerformedOperationPrimaryProcessing
                                                                      {
                                                                          MessageFlowId = flow.Id,
                                                                          UseCaseId = useCaseId,
                                                                          AttemptCount = 0,
                                                                          CreatedOn = currentTime
                                                                      }));
            }

            return store;
        }

        private void PushToQueue(IReadOnlyList<PerformedOperationPrimaryProcessing> primaryProcessings)
        {
            const int BatchSize = 3000;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                for (int i = 0; i < primaryProcessings.Count; i++)
                {
                    _primaryProcessingsRepository.Add(primaryProcessings[i]);
                    if (i != 0 && i % BatchSize == 0)
                    {
                        _primaryProcessingsRepository.Save();
                    }
                }

                _primaryProcessingsRepository.Save();

                transaction.Complete();
            }
        }

        private sealed class ProcessingStatistics
        {
            public double TotalTimeSec { get; set; }
            public int ProcessingCyclesCount { get; set; }
            public int ProcessedElementsCount { get; set; }
        }
    }
}
