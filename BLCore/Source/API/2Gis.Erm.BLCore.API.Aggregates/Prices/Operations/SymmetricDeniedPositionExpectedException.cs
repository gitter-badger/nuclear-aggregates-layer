using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    [Serializable]
    public class SymmetricDeniedPositionExpectedException : ArgumentException
    {
        public SymmetricDeniedPositionExpectedException()
        {
        }

        public SymmetricDeniedPositionExpectedException(string message)
            : base(message)
        {
        }

        public SymmetricDeniedPositionExpectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SymmetricDeniedPositionExpectedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
