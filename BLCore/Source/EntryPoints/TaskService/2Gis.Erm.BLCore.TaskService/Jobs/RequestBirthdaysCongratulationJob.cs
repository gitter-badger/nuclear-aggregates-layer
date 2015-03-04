using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    public sealed class RequestBirthdaysCongratulationJob : TaskServiceJobBase
    {
        private readonly IRequestBirthdayCongratulationOperationService _requestBirthdayCongratulationOperationService;

        public RequestBirthdaysCongratulationJob(
            ICommonLog logger,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            IRequestBirthdayCongratulationOperationService requestBirthdayCongratulationOperationService)
            : base(signInService, userImpersonationService, logger)
        {
            _requestBirthdayCongratulationOperationService = requestBirthdayCongratulationOperationService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _requestBirthdayCongratulationOperationService.AddRequest(DateTime.Today);
        }
    }
}