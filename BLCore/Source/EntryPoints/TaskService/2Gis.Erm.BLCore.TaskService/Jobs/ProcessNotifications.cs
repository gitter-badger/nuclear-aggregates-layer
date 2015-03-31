using DoubleGis.Erm.Platform.API.Core.Notifications;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class ProcessNotifications : TaskServiceJobBase
    {
        private readonly INotificationsProcessor _notificationsProcessor;

        public ProcessNotifications(INotificationsProcessor notificationsProcessor,
                                    ITracer tracer,
                                    ISignInService signInService,
                                    IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _notificationsProcessor = notificationsProcessor;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _notificationsProcessor.Process();
        }
    }
}
