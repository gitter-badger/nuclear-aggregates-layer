using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public abstract class ErmException : Exception
    {
        public ErmException() { }
        public ErmException(String message) : base(message) { }
        public ErmException(String message, Exception innerException) : base(message, innerException) { }
        protected ErmException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
