using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    public class LimitIncreasingAmountException : BusinessLogicException
    {
        public LimitIncreasingAmountException()
        {
        }

        public LimitIncreasingAmountException(string message)
            : base(message)
        {
        }

        public LimitIncreasingAmountException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LimitIncreasingAmountException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
