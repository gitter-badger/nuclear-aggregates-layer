using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.LocalMessages
{
    public sealed class ReprocessLocalMessages : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ReprocessLocalMessages(
            ITracer tracer, 
            IPublicService publicService, 
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }
        
        public int PeriodInMinutes { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (PeriodInMinutes == 0)
            {
                Tracer.ErrorFormat(null, "Задача [{0}] не имеет заданного числа минут для определения зависших сообщений", context.JobDetail.Key.Name);
                return;
            }

            _publicService.Handle(new ReprocessLocalMessagesRequest { PeriodInMinutes = PeriodInMinutes });
        }
    }
}