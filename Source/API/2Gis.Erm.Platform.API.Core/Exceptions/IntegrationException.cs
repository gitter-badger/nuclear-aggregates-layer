using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class IntegrationException : ErmException
    {
        public IntegrationException() { }
        public IntegrationException(String message) : base(message) { }
        public IntegrationException(String message, Exception innerException) : base(message, innerException) { }
        protected IntegrationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}