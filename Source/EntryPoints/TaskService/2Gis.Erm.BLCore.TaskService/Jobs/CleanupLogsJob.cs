﻿using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class CleanupLogsJob : TaskServiceJobBase
    {
        private readonly IDBCleanupSettings _settings;
        private readonly ICleanupPersistenceService _cleanupPersistenceService;

        public CleanupLogsJob(
            ICommonLog logger,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IDBCleanupSettings settings,
            ICleanupPersistenceService cleanupPersistenceService)
            : base(signInService, userImpersonationService, logger)
        {
            _settings = settings;
            _cleanupPersistenceService = cleanupPersistenceService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            // cleanup erm
            _cleanupPersistenceService.CleanupErmLogging(7200, _settings.LogSizeInDays);
            _cleanupPersistenceService.CleanupErm(7200, _settings.LogSizeInDays);
            
            // cleanup crm
            _cleanupPersistenceService.CleanupCrm(7200, _settings.LogSizeInDays);
        }
    }
}
