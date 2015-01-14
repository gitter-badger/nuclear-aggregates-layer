using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    [Serializable]
    public class LimitIsBlockedByReleaseException : BusinessLogicException
    {
        public LimitIsBlockedByReleaseException()
        {
        }

        public LimitIsBlockedByReleaseException(string message)
            : base(message)
        {
        }

        public LimitIsBlockedByReleaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LimitIsBlockedByReleaseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
