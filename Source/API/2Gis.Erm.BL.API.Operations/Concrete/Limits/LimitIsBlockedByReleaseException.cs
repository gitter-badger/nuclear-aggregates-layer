using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Limits
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
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
