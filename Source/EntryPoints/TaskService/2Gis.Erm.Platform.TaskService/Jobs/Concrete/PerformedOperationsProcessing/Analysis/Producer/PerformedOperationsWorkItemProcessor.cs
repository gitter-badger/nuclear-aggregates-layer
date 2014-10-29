using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    public sealed class PerformedOperationsWorkItemProcessor
    {
        private readonly int _processorId;
        private readonly BlockingCollection<PerformedOperationsWorkItem> _workItemsSource;
        private readonly IOperationScopeDisposableFactoryAccessor _operationScopeFactoryAccessor;
        private readonly IIdentityRequestStrategy _identityRequestStrategy;
        private readonly CancellationToken _cancellationToken;
        private readonly ICommonLog _logger;
        private readonly Task _worker;

        public PerformedOperationsWorkItemProcessor(
            int processorId,
            BlockingCollection<PerformedOperationsWorkItem> workItemsSource,
            IOperationScopeDisposableFactoryAccessor operationScopeFactoryAccessor, 
            IIdentityRequestStrategy identityRequestStrategy,
            CancellationToken cancellationToken,
            ICommonLog logger)
        {
            _processorId = processorId;
            _workItemsSource = workItemsSource;
            _operationScopeFactoryAccessor = operationScopeFactoryAccessor;
            _identityRequestStrategy = identityRequestStrategy;
            _cancellationToken = cancellationToken;
            _logger = logger;

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
            _logger.InfoFormatEx("Producing performed operations. Processor id {0} started", _processorId);

            var stopwatch = new Stopwatch();
            while (ContinueProcessing)
            {
                PerformedOperationsWorkItem nextPerformedOperationsWorkItem;
                if (!_workItemsSource.TryTake(out nextPerformedOperationsWorkItem))
                {
                    if (!_workItemsSource.IsCompleted)
                    {
                        _logger.ErrorFormatEx("Producing performed operations. Work items sequence processing aborted by processor with id {0}. Can't take next from not completed sequence" + _processorId);
                    }
                        
                    break;
                }

                for (int i = 0; i < nextPerformedOperationsWorkItem.OperationsCount && ContinueProcessing; i++)
                {
                    _logger.InfoFormatEx(
                        "Producing performed operations. Processor id: {0}. Work item scheduled. Operation count: {1}. Entities count: {2}",
                        _processorId,
                        nextPerformedOperationsWorkItem.OperationsCount,
                        nextPerformedOperationsWorkItem.EntitiesCount);
                        
                    stopwatch.Restart();

                    try
                    {
                        PushOperations(nextPerformedOperationsWorkItem.EntitiesCount);
                        
                        stopwatch.Stop();
                        
                        _logger.InfoFormatEx(
                            "Producing performed operations. Processor id: {0}. Work item processed in {1} sec. Operation count: {2}. Entities count: {3}. Producing rate : {4:F2} op/sec",
                            _processorId,
                            stopwatch.Elapsed.TotalSeconds,
                            nextPerformedOperationsWorkItem.OperationsCount,
                            nextPerformedOperationsWorkItem.EntitiesCount,
                            (double)nextPerformedOperationsWorkItem.OperationsCount / stopwatch.Elapsed.TotalSeconds);  
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        
                        _logger.ErrorFormatEx(
                            ex, 
                            "Producing performed operations. Processor id: {0}. Work item processing failed after {1} sec. Operation count: {2}. Entities count: {3}",
                            _processorId,
                            stopwatch.Elapsed.TotalSeconds,
                            nextPerformedOperationsWorkItem.OperationsCount,
                            nextPerformedOperationsWorkItem.EntitiesCount);
                    }
                }
            }

            _logger.InfoFormatEx("Producing performed operations. Processor id {0} stopped", _processorId);
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