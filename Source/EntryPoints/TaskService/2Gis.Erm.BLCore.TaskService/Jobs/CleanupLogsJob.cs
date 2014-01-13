using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class CleanupLogsJob : TaskServiceJobBase
    {
        private readonly IDbCleaner _dbCleaner;

        public CleanupLogsJob(
            ICommonLog logger,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IDbCleaner dbCLeaner)
            : base(signInService, userImpersonationService, logger)
        {
            _dbCleaner = dbCLeaner;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _dbCleaner.CleanupErm();
            _dbCleaner.CleanupCrm();
        }
    }
}
