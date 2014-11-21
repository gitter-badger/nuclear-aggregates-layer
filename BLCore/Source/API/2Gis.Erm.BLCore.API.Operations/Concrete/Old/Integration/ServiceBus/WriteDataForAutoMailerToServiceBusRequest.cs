using System.IO;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class WriteDataForAutoMailerToServiceBusRequest : ExportRequest
    {
        public Stream MessageStream { get; set; }
    }
}
