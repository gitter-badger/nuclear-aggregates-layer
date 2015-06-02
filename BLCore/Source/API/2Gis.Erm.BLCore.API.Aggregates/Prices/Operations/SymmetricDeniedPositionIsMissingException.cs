using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    [Serializable]
    public class SymmetricDeniedPositionIsMissingException : BusinessLogicException
    {
        public SymmetricDeniedPositionIsMissingException()
        {
        }

        public SymmetricDeniedPositionIsMissingException(string message)
            : base(message)
        {
        }

        public SymmetricDeniedPositionIsMissingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SymmetricDeniedPositionIsMissingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
