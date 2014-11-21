using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import.FlowBilling
{
    public class CannotCreateChargesException : ServiceBusIntegrationException
    {
        public CannotCreateChargesException()
        {
        }

        public CannotCreateChargesException(string message) : base(message)
        {
        }

        public CannotCreateChargesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CannotCreateChargesException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}