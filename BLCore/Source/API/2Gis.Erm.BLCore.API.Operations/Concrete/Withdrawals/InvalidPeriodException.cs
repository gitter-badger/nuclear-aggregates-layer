using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    [Serializable]
    public class InvalidPeriodException : BusinessLogicException
    {
        public InvalidPeriodException()
        {
        }

        public InvalidPeriodException(string message)
            : base(message)
        {
        }

        public InvalidPeriodException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidPeriodException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
