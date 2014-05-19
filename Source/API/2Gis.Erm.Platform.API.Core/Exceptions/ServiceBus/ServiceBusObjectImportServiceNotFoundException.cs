using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus
{
    public class ServiceBusObjectImportServiceNotFoundException : ServiceBusIntegrationException
    {
        public ServiceBusObjectImportServiceNotFoundException()
        {
        }

        public ServiceBusObjectImportServiceNotFoundException(string message) : base(message)
        {
        }

        public ServiceBusObjectImportServiceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ServiceBusObjectImportServiceNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}