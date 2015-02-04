using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.LocalMessages
{
    public sealed class ReprocessLocalMessages : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ReprocessLocalMessages(
            ICommonLog logger, 
            IPublicService publicService, 
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }
        
        public int PeriodInMinutes { get; set; }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            if (PeriodInMinutes == 0)
            {
                Logger.ErrorFormat(null, "Задача [{0}] не имеет заданного числа минут для определения зависших сообщений", context.JobDetail.Key.Name);
                return;
            }

            _publicService.Handle(new ReprocessLocalMessagesRequest { PeriodInMinutes = PeriodInMinutes });
        }
    }
}