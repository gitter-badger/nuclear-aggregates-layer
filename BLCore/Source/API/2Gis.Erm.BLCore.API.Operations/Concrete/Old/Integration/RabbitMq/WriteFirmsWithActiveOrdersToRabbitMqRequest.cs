using System.IO;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.RabbitMq
{
    public sealed class WriteFirmsWithActiveOrdersToRabbitMqRequest : ExportRequest
    {
        public long OrganizationUnitId { get; set; }
        public Stream MessageStream { get; set; }
    }
}