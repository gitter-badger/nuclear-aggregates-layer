using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.RabbitMq
{
    public sealed class ImportLocalMessagesFromRabbitMq : TaskServiceJobBase
    {
        private readonly IPublicService _publicService;

        public ImportLocalMessagesFromRabbitMq(IPublicService publicService, ITracer logger, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, logger)
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
