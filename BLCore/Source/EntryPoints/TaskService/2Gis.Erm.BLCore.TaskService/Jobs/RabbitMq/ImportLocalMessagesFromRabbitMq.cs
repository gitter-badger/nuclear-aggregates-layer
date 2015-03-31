using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.RabbitMq
{
    public sealed class ImportLocalMessagesFromRabbitMq : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ImportLocalMessagesFromRabbitMq(IPublicService publicService, ITracer tracer, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
            _publicService = publicService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _publicService.Handle(new ImportLocalMessagesFromRabbitMqRequest { IntegrationType = IntegrationTypeImport.FirmsFromDgpp, QueueName = "dgpp2erm.firm" });
            _publicService.Handle(new ImportLocalMessagesFromRabbitMqRequest { IntegrationType = IntegrationTypeImport.TerritoriesFromDgpp, QueueName = "dgpp2erm.territory" });
        }
    }
}
