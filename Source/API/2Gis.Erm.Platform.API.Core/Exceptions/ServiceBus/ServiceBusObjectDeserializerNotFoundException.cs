using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus
{
    public class ServiceBusObjectDeserializerNotFoundException : ServiceBusIntegrationException
    {
        public ServiceBusObjectDeserializerNotFoundException()
        {
        }

        public ServiceBusObjectDeserializerNotFoundException(string message) : base(message)
        {
        }

        public ServiceBusObjectDeserializerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ServiceBusObjectDeserializerNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}