using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import
{
    public class NonBlockingImportErrorException : ServiceBusIntegrationException
    {
        public NonBlockingImportErrorException()
        {
        }

        public NonBlockingImportErrorException(string message) : base(message)
        {
        }

        public NonBlockingImportErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NonBlockingImportErrorException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}