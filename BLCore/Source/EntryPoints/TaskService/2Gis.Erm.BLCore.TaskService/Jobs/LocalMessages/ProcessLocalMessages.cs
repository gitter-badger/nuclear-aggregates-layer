using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.LocalMessages
{
    [DisallowConcurrentExecution]
    public sealed class ProcessLocalMessages : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ProcessLocalMessages(ICommonLog logger, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ProcessLocalMessagesRequest());
        }
    }
}