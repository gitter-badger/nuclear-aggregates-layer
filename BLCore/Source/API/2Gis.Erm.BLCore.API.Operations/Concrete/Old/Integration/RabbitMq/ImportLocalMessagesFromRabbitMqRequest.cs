using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq
{
    public sealed class ImportLocalMessagesFromRabbitMqRequest : Request
    {
        public IntegrationTypeImport IntegrationType { get; set; }
        public string QueueName { get; set; }
    }
}