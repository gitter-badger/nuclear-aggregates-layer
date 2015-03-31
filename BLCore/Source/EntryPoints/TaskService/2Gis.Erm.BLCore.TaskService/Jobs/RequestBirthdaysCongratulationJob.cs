using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class RequestBirthdaysCongratulationJob : TaskServiceJobBase
    {
        private readonly IRequestBirthdayCongratulationOperationService _requestBirthdayCongratulationOperationService;

        public RequestBirthdaysCongratulationJob(
            ITracer tracer,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IRequestBirthdayCongratulationOperationService requestBirthdayCongratulationOperationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _requestBirthdayCongratulationOperationService = requestBirthdayCongratulationOperationService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _requestBirthdayCongratulationOperationService.AddRequest(DateTime.Today);
        }
    }
}