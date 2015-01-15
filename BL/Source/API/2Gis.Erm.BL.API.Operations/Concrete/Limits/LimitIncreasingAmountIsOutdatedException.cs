using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    public class LimitIncreasingAmountIsOutdatedException : BusinessLogicException
    {
        public LimitIncreasingAmountIsOutdatedException()
        {
        }

        public LimitIncreasingAmountIsOutdatedException(string message)
            : base(message)
        {
        }

        public LimitIncreasingAmountIsOutdatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LimitIncreasingAmountIsOutdatedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}