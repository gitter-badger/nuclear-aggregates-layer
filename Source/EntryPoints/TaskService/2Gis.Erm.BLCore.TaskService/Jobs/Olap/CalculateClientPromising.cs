using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.Olap
{
    public sealed class CalculateClientPromising : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public CalculateClientPromising(ICommonLog logger, IPublicService publicService, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new CalculateClientPromisingRequest());
        }
    }
}
