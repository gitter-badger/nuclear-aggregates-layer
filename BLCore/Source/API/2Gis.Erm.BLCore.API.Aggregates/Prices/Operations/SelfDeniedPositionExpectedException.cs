using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    [Serializable]
    public class SelfDeniedPositionExpectedException : ArgumentException
    {
        public SelfDeniedPositionExpectedException()
        {
        }

        public SelfDeniedPositionExpectedException(string message)
            : base(message)
        {
        }

        public SelfDeniedPositionExpectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SelfDeniedPositionExpectedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
