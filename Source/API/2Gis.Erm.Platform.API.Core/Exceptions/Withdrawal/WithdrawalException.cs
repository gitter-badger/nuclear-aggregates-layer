using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal
{
    public class WithdrawalException : BusinessLogicException
    {
        public WithdrawalException()
        {
        }

        public WithdrawalException(string message) : base(message)
        {
        }

        public WithdrawalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WithdrawalException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}