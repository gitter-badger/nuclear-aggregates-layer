using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific;
using NuClear.IdentityService.Client.Interaction;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    public sealed class PerformedOperationsWorkItemProcessor
    {
        private readonly int _processorId;
        private readonly BlockingCollection<PerformedOperationsWorkItem> _workItemsSource;
        private readonly IOperationScopeDisposableFactoryAccessor _operationScopeFactoryAccessor;
        private readonly IIdentityRequestStrategy _identityRequestStrategy;
        private readonly CancellationToken _cancellationToken;
        private readonly ITracer _tracer;
        private readonly Task _worker;

        public PerformedOperationsWorkItemProcessor(
            int processorId,
            BlockingCollection<PerformedOperationsWorkItem> workItemsSource,
            IOperationScopeDisposableFactoryAccessor operationScopeFactoryAccessor, 
            IIdentityRequestStrategy identityRequestStrategy,
            CancellationToken cancellationToken,
            ITracer tracer)
        {
            _processorId = processorId;
            _workItemsSource = workItemsSource;
            _operationScopeFactoryAccessor = operationScopeFactoryAccessor;
            _identityRequestStrategy = identityRequestStrategy;
            _cancellationToken = cancellationToken;
            _tracer = tracer;

            _worker = new Task(WorkerFunc, TaskCreationOptions.LongRunning);
        }
            
        private bool ContinueProcessing
        {
            get { return !_cancellationToken.IsCancellationRequested && !_workItemsSource.IsCompleted; }
        }
            
        public Task Process()
        {
            _worker.Start();
            return _worker;
        }

        private void WorkerFunc()
        {
            _tracer.InfoFormat("Producing performed operations. Processor id {0} started", _processorId);

            var workItemStopwatch = new Stopwatch();
            var operationStopwatch = new Stopwatch();
            
            while (ContinueProcessing)
            {
                PerformedOperationsWorkItem nextPerformedOperationsWorkItem;
                if (!_workItemsSource.TryTake(out nextPerformedOperationsWorkItem, 1000))
                {
                    continue;
                }

                workItemStopwatch.Restart();

                int succeeded = 0;
                int failed = 0;
                
                _tracer.InfoFormat(
                        "Producing performed operations. Processor id: {0}. Work item scheduled. Operation count: {1}. Entities count: {2}",
                        _processorId,
                        nextPerformedOperationsWorkItem.OperationsCount,
                        nextPerformedOperationsWorkItem.EntitiesCount);

                for (int i = 0; i < nextPerformedOperationsWorkItem.OperationsCount && ContinueProcessing; i++)
                {
                    operationStopwatch.Restart();

                    try
                    {
                        PushOperations(nextPerformedOperationsWorkItem.EntitiesCount);
                        
                        operationStopwatch.Stop();
                        ++succeeded;
                        
                        _tracer.DebugFormat(
                            "Producing performed operations. Processor id: {0}. Operation pushed in {1} sec. Operation count: {2}. Entities count: {3}. Producing rate : {4:F2} op/sec. Units (ops|entities) rate: {5:F2} unit/sec",
                            _processorId,
                            operationStopwatch.Elapsed.TotalSeconds,
                            nextPerformedOperationsWorkItem.OperationsCount,
                            nextPerformedOperationsWorkItem.EntitiesCount,
                            1 / operationStopwatch.Elapsed.TotalSeconds,
                            (double)(nextPerformedOperationsWorkItem.EntitiesCount + 1) / operationStopwatch.Elapsed.TotalSeconds);  
                    }
                    catch (Exception ex)
                    {
                        operationStopwatch.Stop();
                        ++failed;
                        
                        _tracer.ErrorFormat(
                            ex, 
                            "Producing performed operations. Processor id: {0}. Operation push failed after {1} sec. Operation count: {2}. Entities count: {3}",
                            _processorId,
                            operationStopwatch.Elapsed.TotalSeconds,
                            nextPerformedOperationsWorkItem.OperationsCount,
                            nextPerformedOperationsWorkItem.EntitiesCount);
                    }
                }

                workItemStopwatch.Stop();

                _tracer.InfoFormat(
                        "Producing performed operations. Processor id: {0}. Work item processed in {1:F2} sec by ratio {2:F2}%, succeeded {3:F2}%. Operation count: {4}. Entities count: {5}. Producing rate : {6:F2} op/sec. Units (ops|entities) rate: {7:F2} unit/sec",
                        _processorId,
                        workItemStopwatch.Elapsed.TotalSeconds,
                        (double)(succeeded + failed) * 100 / nextPerformedOperationsWorkItem.OperationsCount,
                        (double)succeeded * 100 / (failed + succeeded),
                        nextPerformedOperationsWorkItem.OperationsCount,
                        nextPerformedOperationsWorkItem.EntitiesCount,
                        succeeded / operationStopwatch.Elapsed.TotalSeconds,
                        (double)(nextPerformedOperationsWorkItem.EntitiesCount + 1) * nextPerformedOperationsWorkItem .OperationsCount / operationStopwatch.Elapsed.TotalSeconds);
            }

            _tracer.InfoFormat("Producing performed operations. Processor id {0} stopped", _processorId);
        }

        private void PushOperations(int affectedEntitiesCount)
        {
            using (var scopeFactory = _operationScopeFactoryAccessor.Factory)
            using (var scope = scopeFactory.CreateNonCoupled<PerformedOperationProcessingAnalysisIdentity>())
            {
                if (affectedEntitiesCount > 0)
                {
                    using (var nestedScope = scopeFactory.CreateSpecificFor<CreateIdentity, PerformedOperationPrimaryProcessing>())
                    {
                        var generatedIds = _identityRequestStrategy.Request(affectedEntitiesCount);
                        nestedScope.Updated<PerformedOperationPrimaryProcessing>(generatedIds)
                                    .Complete();
                    }
                }

                scope.Complete();
            }
        }
    }
}