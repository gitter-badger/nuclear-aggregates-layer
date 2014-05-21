using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import.FlowBilling
{
    public class CannotAcquireOrderPositionsForChargesException : ServiceBusIntegrationException
    {
        public CannotAcquireOrderPositionsForChargesException(string message) : base(message)
        {
        }

        public CannotAcquireOrderPositionsForChargesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CannotAcquireOrderPositionsForChargesException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public CannotAcquireOrderPositionsForChargesException()
        {
        }
    }
}