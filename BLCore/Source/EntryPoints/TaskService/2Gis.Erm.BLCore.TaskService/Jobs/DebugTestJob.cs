using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class DebugTestJob : TaskServiceJobBase
    {
        public DebugTestJob(ITracer tracer, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Console.WriteLine("Enter DebugTestJob, time: {0}", DateTime.UtcNow);
            Thread.Sleep(5000);
            Console.WriteLine("Exit DebugTestJob, time: {0}", DateTime.UtcNow);
        }
    }
}