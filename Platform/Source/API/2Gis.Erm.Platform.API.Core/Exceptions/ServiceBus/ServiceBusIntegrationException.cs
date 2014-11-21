using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus
{
    public abstract class ServiceBusIntegrationException : ErmException
    {
        protected ServiceBusIntegrationException()
        {
        }

        protected ServiceBusIntegrationException(string message)
            : base(message)
        {
        }

        protected ServiceBusIntegrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ServiceBusIntegrationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}