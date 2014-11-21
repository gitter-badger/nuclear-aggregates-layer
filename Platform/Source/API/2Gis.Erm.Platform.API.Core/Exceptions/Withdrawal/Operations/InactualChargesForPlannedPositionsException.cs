using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations
{
    public class InactualChargesForPlannedPositionsException : WithdrawalException
    {
        public InactualChargesForPlannedPositionsException()
        {
        }

        public InactualChargesForPlannedPositionsException(string message) : base(message)
        {
        }

        public InactualChargesForPlannedPositionsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InactualChargesForPlannedPositionsException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}