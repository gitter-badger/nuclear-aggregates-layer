using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;
using NuClear.Jobs;

using NuClear.Tracing.API;

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