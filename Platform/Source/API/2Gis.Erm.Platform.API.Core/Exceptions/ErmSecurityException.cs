using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public class ErmSecurityException : ErmException
    {
        public ErmSecurityException() { }
        public ErmSecurityException(String message) : base(message) { }
        public ErmSecurityException(String message, Exception innerException) : base(message, innerException) { }
        protected ErmSecurityException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}
