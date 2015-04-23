using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.LocalMessages
{
    [DisallowConcurrentExecution]
    public sealed class ProcessLocalMessages : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ProcessLocalMessages(ITracer tracer, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ProcessLocalMessagesRequest());
        }
    }
}