using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    [Serializable]
    public class SpecifyOrderDocumentsDebtOperationIsBlockedByReleaseException : BusinessLogicException
    {
        public SpecifyOrderDocumentsDebtOperationIsBlockedByReleaseException()
        {
        }

        public SpecifyOrderDocumentsDebtOperationIsBlockedByReleaseException(string message)
            : base(message)
        {
        }

        public SpecifyOrderDocumentsDebtOperationIsBlockedByReleaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SpecifyOrderDocumentsDebtOperationIsBlockedByReleaseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
