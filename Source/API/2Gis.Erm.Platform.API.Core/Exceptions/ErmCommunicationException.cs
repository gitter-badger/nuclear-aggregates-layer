using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class ErmCommunicationException : ErmException
    {
        public ErmCommunicationException() { }
        public ErmCommunicationException(String message) : base(message) { }
        public ErmCommunicationException(String message, Exception innerException) : base(message, innerException) { }
        protected ErmCommunicationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}