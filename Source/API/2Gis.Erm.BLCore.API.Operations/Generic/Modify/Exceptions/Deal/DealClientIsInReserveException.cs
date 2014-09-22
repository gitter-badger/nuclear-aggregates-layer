using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Exceptions.Deal
{
    [Serializable]
    public class DealClientIsInReserveException : BusinessLogicException
    {
        public DealClientIsInReserveException()
        {
        }

        public DealClientIsInReserveException(string message)
            : base(message)
        {
        }

        public DealClientIsInReserveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DealClientIsInReserveException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}