using System.Collections.Generic;
using System.Diagnostics;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing.Analysis
{
    [DisallowConcurrentExecution]
    public sealed class PerformedOperationsProducerJob : TaskServiceJobBase, IInterruptableJob
    {
        private const int MaxContextChangesCount = 265000;

        private readonly IIdentityRequestStrategy _identityRequestStrategy;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IReadOnlyList<WorkItem> _operationsProfiles;
        
        private bool _isStopped;

        public PerformedOperationsProducerJob(
            IIdentityRequestStrategy identityRequestStrategy,
            IOperationScopeFactory scopeFactory,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ICommonLog logger)
            : base(signInService, userImpersonationService, logger)
        {
            _identityRequestStrategy = identityRequestStrategy;
            _scopeFactory = scopeFactory;

            _operationsProfiles = new[]
                                     {
                                         new WorkItem { OperationsCount = 1000, EntitiesCount = 1 },
                                         new WorkItem { OperationsCount = 300, EntitiesCount = 100 },
                                         new WorkItem { OperationsCount = 200, EntitiesCount = 1000 },
                                         new WorkItem { OperationsCount = 100, EntitiesCount = 5000 },
                                         new WorkItem { OperationsCount = 50, EntitiesCount = 10000 },
                                         new WorkItem { OperationsCount = 3, EntitiesCount = 100000 },
                                         new WorkItem { OperationsCount = 1, EntitiesCount = MaxContextChangesCount }
                                     };
        }

        public void Interrupt()
        {
            Logger.InfoEx("Producing performed operations. Interrupt called for job, producing performed operations is stopping");
            _isStopped = true;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            var sequenceStopWatch = new Stopwatch();
            var workItemStopwatch = new Stopwatch();

            Logger.InfoEx("Producing performed operations. Processing started");

            for (int i = 0; !_isStopped; i++)
            {
                sequenceStopWatch.Restart();
                
                Logger.InfoFormatEx("Producing performed operations. Profiles processing iteration {0} started", i);

                for (int j = 0; j < _operationsProfiles.Count && !_isStopped; j++)
                {
                    var operationProfile = _operationsProfiles[i];
                    Logger.InfoFormatEx(
                        "Producing performed operations. Profile processing scheduled. Operation count: {0}. Entities count: {1}",
                        operationProfile.OperationsCount,
                        operationProfile.EntitiesCount);

                    workItemStopwatch.Restart();
                    PushOperations(operationProfile);
                    workItemStopwatch.Stop();

                    Logger.InfoFormatEx(
                        "Producing performed operations. Profile processed in {0} sec. Operation count: {1}. Entities count: {2}. Producing rate : {3:F2} op/sec",
                        workItemStopwatch.Elapsed.TotalSeconds,
                        operationProfile.OperationsCount,
                        operationProfile.EntitiesCount,
                        (double)operationProfile.OperationsCount / workItemStopwatch.Elapsed.TotalSeconds);  
                }

                sequenceStopWatch.Stop();
                Logger.InfoFormatEx("Producing performed operations. Profiles processing iteration {0} finished in {1} sec", i, sequenceStopWatch.Elapsed.TotalSeconds);
            }

            Logger.InfoEx("Producing performed operations. Processing stopped");
        }

        private void PushOperations(WorkItem workItem)
        {
            for (int i = 0; i < workItem.OperationsCount && !_isStopped; i++)
            {
                using (var scope = _scopeFactory.CreateNonCoupled<PerformedOperationProcessingAnalysisIdentity>())
                {
                    if (workItem.EntitiesCount > 0)
                    {
                        using (var nestedScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
                        {
                            var generatedIds = _identityRequestStrategy.Request(workItem.EntitiesCount);
                            nestedScope.Updated<FirmAddress>(generatedIds)
                                       .Complete();
                        }
                    }

                    scope.Complete();
                }
            }
        }

        private sealed class WorkItem
        {
            public int OperationsCount { get; set; }
            public int EntitiesCount { get; set; }
        }
    }
}