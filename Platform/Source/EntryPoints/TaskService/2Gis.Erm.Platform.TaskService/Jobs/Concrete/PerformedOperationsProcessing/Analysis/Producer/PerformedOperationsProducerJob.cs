using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Identities;
using NuClear.IdentityService.Client.Interaction;
using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    [DisallowConcurrentExecution]
    public sealed class PerformedOperationsProducerJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly IOperationScopeDisposableFactoryAccessor _operationScopeFactoryAccessor;
        private readonly IIdentityRequestStrategy _identityRequestStrategy;
        private readonly IReadOnlyList<PerformedOperationsWorkItem> _operationsProfiles;
        
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _workItemsGenerator;
        private readonly BlockingCollection<PerformedOperationsWorkItem> _workItemsSource;

        public PerformedOperationsProducerJob(
            IOperationScopeDisposableFactoryAccessor operationScopeFactoryAccessor,
            IIdentityRequestStrategy identityRequestStrategy,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _operationScopeFactoryAccessor = operationScopeFactoryAccessor;
            _identityRequestStrategy = identityRequestStrategy;

            _cancellationTokenSource = new CancellationTokenSource();

            _operationsProfiles = new[]
                                     {
                                         new PerformedOperationsWorkItem { OperationsCount = 1000, EntitiesCount = 1 },
                                         new PerformedOperationsWorkItem { OperationsCount = 300, EntitiesCount = 100 },
                                         new PerformedOperationsWorkItem { OperationsCount = 200, EntitiesCount = 1000 },
                                         new PerformedOperationsWorkItem { OperationsCount = 100, EntitiesCount = 5000 },
                                         new PerformedOperationsWorkItem { OperationsCount = 50, EntitiesCount = 10000 },
                                         new PerformedOperationsWorkItem { OperationsCount = 10, EntitiesCount = 100000 },
                                         new PerformedOperationsWorkItem { OperationsCount = 7, EntitiesCount = 250000 },
                                         new PerformedOperationsWorkItem { OperationsCount = 5, EntitiesCount = 500000 },
                                         //new PerformedOperationsWorkItem { OperationsCount = 3, EntitiesCount = 1000000 },
                                         //new PerformedOperationsWorkItem { OperationsCount = 1, EntitiesCount = 5000000 }
                                     };

            _workItemsGenerator = new Task(WorkItemsGeneratorFunc, TaskCreationOptions.LongRunning);
            _workItemsSource = new BlockingCollection<PerformedOperationsWorkItem>();
        }

        public int MaxParallelism { get; set; }

        public void Interrupt()
        {
            Tracer.Info("Producing performed operations. Interrupt called for job, producing performed operations is stopping");
            _cancellationTokenSource.Cancel();
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Tracer.Info("Producing performed operations. Processing started. Processors used: " + MaxParallelism);

            var workItemsProcessors = new Task[MaxParallelism];
            for (int i = 0; i < workItemsProcessors.Length; i++)
            {
                var processor = new PerformedOperationsWorkItemProcessor(i, _workItemsSource, _operationScopeFactoryAccessor, _identityRequestStrategy, _cancellationTokenSource.Token, Tracer);
                workItemsProcessors[i] = processor.Process();
            }

            _workItemsGenerator.Start();
            Task.WaitAll(workItemsProcessors);
            
            Tracer.Info("Producing performed operations. Processing stopped");
        }

        private void WorkItemsGeneratorFunc()
        {
            Tracer.Info("Producing performed operations. Work items generator started");

            var cancellationToken = _cancellationTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_workItemsSource.Count < MaxParallelism)
                {
                    foreach (var operationsProfile in _operationsProfiles)
                    {
                        _workItemsSource.Add(operationsProfile);
                    }
                }

                Thread.Sleep(500);
            }

            _workItemsSource.CompleteAdding();

            Tracer.Info("Producing performed operations. Work items generator stopped");
        }
    }
}