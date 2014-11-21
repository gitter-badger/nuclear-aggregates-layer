using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    [Serializable]
    public abstract class ErmException : ApplicationException
    {
        protected ErmException()
        {
        }

        protected ErmException(String message) : base(message)
        {
        }

        protected ErmException(String message, Exception innerException) : base(message, innerException)
        {
        }

        protected ErmException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}