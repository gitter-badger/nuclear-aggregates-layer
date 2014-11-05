using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    [Serializable]
    public class LimitAmountException : BusinessLogicException
    {
        public LimitAmountException()
        {
        }

        public LimitAmountException(string message)
            : base(message)
        {
        }

        public LimitAmountException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LimitAmountException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
