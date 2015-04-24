using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.UserProfiles;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.ADSync
{
    public sealed class SyncUserProfiles : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public SyncUserProfiles(ITracer tracer, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new SyncUserProfilesRequest());
        }
    }
}
