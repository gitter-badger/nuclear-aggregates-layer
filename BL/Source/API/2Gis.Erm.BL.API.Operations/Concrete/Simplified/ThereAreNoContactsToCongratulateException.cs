using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Simplified
{
    [Serializable]
    public class ThereAreNoContactsToCongratulateException : BusinessLogicException
    {
        public ThereAreNoContactsToCongratulateException()
        {
        }

        public ThereAreNoContactsToCongratulateException(string message)
            : base(message)
        {
        }

        public ThereAreNoContactsToCongratulateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ThereAreNoContactsToCongratulateException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}