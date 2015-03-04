using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

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
