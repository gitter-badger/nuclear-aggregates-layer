using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.Aggregates
{
    [Serializable]
    public sealed class ProcessAccountsWithDebtsException : Exception
    {
        public ProcessAccountsWithDebtsException() { }
        public ProcessAccountsWithDebtsException(string message) : base(message) { }
        public ProcessAccountsWithDebtsException(string message, Exception innerException) : base(message, innerException) { }
        private ProcessAccountsWithDebtsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}