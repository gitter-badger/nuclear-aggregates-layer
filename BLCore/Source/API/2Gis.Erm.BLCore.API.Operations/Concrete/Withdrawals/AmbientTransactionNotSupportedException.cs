using System;
using System.Runtime.Serialization;
using System.Transactions;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    [Serializable]
    public class AmbientTransactionNotSupportedException : TransactionException
    {
        public AmbientTransactionNotSupportedException()
        {
        }

        public AmbientTransactionNotSupportedException(string message)
            : base(message)
        {
        }

        public AmbientTransactionNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AmbientTransactionNotSupportedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}
