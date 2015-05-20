using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    [Serializable]
    public class NonSelfDeniedPositionExpectedException : ArgumentException
    {
        public NonSelfDeniedPositionExpectedException()
        {
        }

        public NonSelfDeniedPositionExpectedException(string message)
            : base(message)
        {
        }

        public NonSelfDeniedPositionExpectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NonSelfDeniedPositionExpectedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
