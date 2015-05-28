using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.HotClient;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Storage;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    public sealed class PrimaryProcessingCUDOnlySQLTransportLoadTest : IIntegrationTest
    {
        private readonly IOperationsPrimaryProcessingAbandonAggregateService _operationsPrimaryProcessingAbandonAggregateService;
        private readonly IOperationsPrimaryProcessingCompleteAggregateService _operationsPrimaryProcessingCompleteAggregateService;

        private readonly IReadOnlyCollection<Guid> _targetMessageFlows = new[]
                                                                    {
                                                                        PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance.Id,
                                                                        PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance.Id,
                                                                        PrimaryProcessHotClientPerformedOperationsFlow.Instance.Id
                                                                    };
        private readonly IRepository<PerformedOperationPrimaryProcessing> _primaryProcessingsRepository;
        private readonly ITracer _tracer;

        public PrimaryProcessingCUDOnlySQLTransportLoadTest(
            IRepository<PerformedOperationPrimaryProcessing> primaryProcessingsRepository,
            IOperationsPrimaryProcessingAbandonAggregateService operationsPrimaryProcessingAbandonAggregateService,
            IOperationsPrimaryProcessingCompleteAggregateService operationsPrimaryProcessingCompleteAggregateService,
            ITracer tracer)
        {
            _primaryProcessingsRepository = primaryProcessingsRepository;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingAbandonAggregateService = operationsPrimaryProcessingAbandonAggregateService;
            _operationsPrimaryProcessingCompleteAggregateService = operationsPrimaryProcessingCompleteAggregateService;
            _tracer = tracer;
        }

        public ITestResult Execute()
        {
            const int IterationsCount = 4;
            const int WarmUpIterationsCount = 1;
            const int TestSequenceLength = 100000;
            
            var enqueueTimes = new List<double>();
            var updateTimes = new List<double>();
            var dequeueTimes = new List<double>();

            for (int i = 0; i < IterationsCount; i++)
            {
                var primaryProcessings = CreatePrimaryProcessings(TestSequenceLength);

                var stopwatch = Stopwatch.StartNew();
                PushToQueue(primaryProcessings);
                stopwatch.Stop();
                if (i >= WarmUpIterationsCount)
                {
                    enqueueTimes.Add(stopwatch.Elapsed.TotalSeconds);
                }

                _tracer.InfoFormat("Push to queue finished and it takes {0} sec", stopwatch.Elapsed.TotalSeconds);

                stopwatch.Restart();
                _operationsPrimaryProcessingAbandonAggregateService.Abandon(primaryProcessings);
                stopwatch.Stop();
                if (i >= WarmUpIterationsCount)
                {
                    updateTimes.Add(stopwatch.Elapsed.TotalSeconds);
                }

                _tracer.InfoFormat("Update queue elements finished and it takes {0} sec", stopwatch.Elapsed.TotalSeconds);

                stopwatch.Restart();
                _operationsPrimaryProcessingCompleteAggregateService.CompleteProcessing(primaryProcessings);
                stopwatch.Stop();
                if (i >= WarmUpIterationsCount)
                {
                    dequeueTimes.Add(stopwatch.Elapsed.TotalSeconds);
                }

                _tracer.InfoFormat("Delete from queue finished and it takes {0} sec", stopwatch.Elapsed.TotalSeconds);
            }

            int effectiveQueueLength = TestSequenceLength * _targetMessageFlows.Count;

            double avgEnqueueTime = enqueueTimes.Average();
            double avgUpdateTime = updateTimes.Average();
            double avgDequeueTime = dequeueTimes.Average();

            var reportBuilder = new StringBuilder();
            reportBuilder.AppendFormat("All {0}({1} - warmup) iterations finished. Test sequence length: {2}. {3}",
                                       IterationsCount,
                                       WarmUpIterationsCount,
                                       effectiveQueueLength,
                                       Environment.NewLine);
            reportBuilder.AppendFormat("Enqueue. {0}{1}", PrepareResultsReport(enqueueTimes, effectiveQueueLength, out avgEnqueueTime), Environment.NewLine);
            reportBuilder.AppendFormat("Update. {0}{1}", PrepareResultsReport(updateTimes, effectiveQueueLength, out avgUpdateTime), Environment.NewLine);
            reportBuilder.AppendFormat("Dequeue. {0}{1}", PrepareResultsReport(dequeueTimes, effectiveQueueLength, out avgDequeueTime), Environment.NewLine);
            var totalTimeSec = avgEnqueueTime + avgUpdateTime + avgDequeueTime;
            reportBuilder.AppendFormat("Total avg time {0} sec. Rate: {1:N2} entry/sec{2}", totalTimeSec, effectiveQueueLength / totalTimeSec, Environment.NewLine);

            _tracer.Info("Test report: " + reportBuilder);

            return OrdinaryTestResult.As.Succeeded;
        }

        private string PrepareResultsReport(IReadOnlyList<double> measuredResults, int processedEntriesCount, out double avgTime)
        {
            avgTime = measuredResults.Average();

            return string.Format(
                "Avg time {0} sec. Rate: {1:N2} entry/sec. Iterations results: {2}", 
                avgTime,
                processedEntriesCount / avgTime,
                measuredResults.Aggregate(new StringBuilder(), (builder, value) => builder.AppendFormat("{0}; ", value)));
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
                                                                          MessageFlowId = flow,
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

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
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
    }
}
