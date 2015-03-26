using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Jobs;

using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.LocalMessages
{
    public sealed class ExportLocalMessages : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ExportLocalMessages(
            IPublicService publicService, 
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService,
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ExportLocalMessageRequest { IntegrationType = IntegrationTypeExport.FirmsWithActiveOrdersToDgpp });
        }
    }
}