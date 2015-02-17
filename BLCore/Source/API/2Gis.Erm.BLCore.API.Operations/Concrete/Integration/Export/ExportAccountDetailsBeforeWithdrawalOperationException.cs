using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export
{
    [Serializable]
    public class ExportAccountDetailsBeforeWithdrawalOperationException : BusinessLogicException
    {
        public ExportAccountDetailsBeforeWithdrawalOperationException()
        {
        }

        public ExportAccountDetailsBeforeWithdrawalOperationException(string message)
            : base(message)
        {
        }

        public ExportAccountDetailsBeforeWithdrawalOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ExportAccountDetailsBeforeWithdrawalOperationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}