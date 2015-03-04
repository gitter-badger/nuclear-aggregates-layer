using System;

using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class CleanupLogsJob : TaskServiceJobBase
    {
        private readonly IDBCleanupSettings _settings;
        private readonly ICleanupPersistenceService _cleanupPersistenceService;

        public CleanupLogsJob(
            ITracer tracer,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IDBCleanupSettings settings,
            ICleanupPersistenceService cleanupPersistenceService)
            : base(signInService, userImpersonationService, tracer)
        {
            _settings = settings;
            _cleanupPersistenceService = cleanupPersistenceService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            // cleanup erm

            // TODO {all, 09.04.2014}: раскомментировать или удалить после решения тикета ERM-3841
            // _cleanupPersistenceService.CleanupErmLogging(7200, _settings.LogSizeInDays);
            _cleanupPersistenceService.CleanupErm(TimeSpan.FromHours(2), _settings.LogSizeInDays);
            
            // cleanup crm
            _cleanupPersistenceService.CleanupCrm(TimeSpan.FromHours(2), _settings.LogSizeInDays);
        }
    }
}
