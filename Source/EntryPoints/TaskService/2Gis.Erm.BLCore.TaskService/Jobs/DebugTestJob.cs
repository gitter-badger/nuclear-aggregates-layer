using System;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class DebugTestJob : TaskServiceJobBase
    {
        public DebugTestJob(ICommonLog logger, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Console.WriteLine("DebugTestJob, time: {0}", DateTime.UtcNow);
        }
    }
}