using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    [Serializable]
    public class AgencyFeeFormatException : BusinessLogicException
    {
        public AgencyFeeFormatException()
        {
        }

        public AgencyFeeFormatException(string message)
            : base(message)
        {
        }

        public AgencyFeeFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AgencyFeeFormatException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
