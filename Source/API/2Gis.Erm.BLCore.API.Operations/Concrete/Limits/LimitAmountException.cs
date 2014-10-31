using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Limits
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
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
