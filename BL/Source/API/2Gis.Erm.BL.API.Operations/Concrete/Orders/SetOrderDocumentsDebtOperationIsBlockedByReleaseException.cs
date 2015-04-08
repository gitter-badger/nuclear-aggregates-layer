using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Orders
{
    [Serializable]
    public class SetOrderDocumentsDebtOperationIsBlockedByReleaseException : OperationException<Order, SetOrderDocumentsDebtIdentity>
    {
        public SetOrderDocumentsDebtOperationIsBlockedByReleaseException()
        {
        }

        public SetOrderDocumentsDebtOperationIsBlockedByReleaseException(string message)
            : base(message)
        {
        }

        public SetOrderDocumentsDebtOperationIsBlockedByReleaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SetOrderDocumentsDebtOperationIsBlockedByReleaseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
