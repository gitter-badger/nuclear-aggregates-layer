using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Jobs;

using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.Olap
{
    [DisallowConcurrentExecution]
    public sealed class ImportFirmPromising : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ImportFirmPromising(ITracer tracer, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ImportFirmPromisingRequest());
        }
    }
}