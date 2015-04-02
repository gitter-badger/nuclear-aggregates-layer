using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    [Serializable]
    public class SymmetricDeniedPositionRuleIsMissingException : BusinessLogicException
    {
        public SymmetricDeniedPositionRuleIsMissingException()
        {
        }

        public SymmetricDeniedPositionRuleIsMissingException(string message)
            : base(message)
        {
        }

        public SymmetricDeniedPositionRuleIsMissingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SymmetricDeniedPositionRuleIsMissingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
