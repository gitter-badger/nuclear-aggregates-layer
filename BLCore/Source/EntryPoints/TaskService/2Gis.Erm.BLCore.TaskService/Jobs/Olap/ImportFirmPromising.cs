using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.Olap
{
    [DisallowConcurrentExecution]
    public sealed class ImportFirmPromising : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ImportFirmPromising(ICommonLog logger, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ImportFirmPromisingRequest());
        }
    }
}