using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations
{
    public class MissingChargesForProjectException : BusinessLogicException
    {
        public MissingChargesForProjectException()
        {
        }

        public MissingChargesForProjectException(string message) : base(message)
        {
        }

        public MissingChargesForProjectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingChargesForProjectException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}