using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations
{
    public class MissingChargesForPlannedPositionsException : BusinessLogicException
    {
        public MissingChargesForPlannedPositionsException()
        {
        }

        public MissingChargesForPlannedPositionsException(string message) : base(message)
        {
        }

        public MissingChargesForPlannedPositionsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingChargesForPlannedPositionsException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}