using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations
{
    public class MultipleChargesSessionsFoundException : BusinessLogicException
    {
        public MultipleChargesSessionsFoundException()
        {
        }

        public MultipleChargesSessionsFoundException(string message) : base(message)
        {
        }

        public MultipleChargesSessionsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipleChargesSessionsFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}