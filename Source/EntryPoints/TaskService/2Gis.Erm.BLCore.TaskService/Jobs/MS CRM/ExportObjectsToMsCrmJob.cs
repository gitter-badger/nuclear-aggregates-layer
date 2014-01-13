using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    // FIXME {all, 15.09.2013}: непонятно что за заготовка, нужна она или нет
    [DisallowConcurrentExecution]
    public sealed class ExportObjectsToMsCrmJob : TaskServiceJobBase
    {
        public ExportObjectsToMsCrmJob(ICommonLog logger,
                                       ISignInService signInService,
                                       IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
        }

        public bool ExportInvalidObjects { get; set; }
        public int OperationCount { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {

        }
    }
}
