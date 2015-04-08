using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    public class EntityIsInReserveException : BusinessLogicException
    {
        public EntityIsInReserveException()
        {
        }

        public EntityIsInReserveException(string message) : base(message)
        {
        }

        public EntityIsInReserveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityIsInReserveException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}