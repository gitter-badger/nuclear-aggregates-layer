using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    [Serializable]
    public class AccountingMethodViolationException : ArgumentException
    {
        public AccountingMethodViolationException(AccountingMethod accountingMethod)
            : base(GetMessage(accountingMethod))
        {
        }

        public AccountingMethodViolationException(AccountingMethod accountingMethod, Exception innerException)
            : base(GetMessage(accountingMethod), innerException)
        {
        }

        protected AccountingMethodViolationException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        private static string GetMessage(AccountingMethod accountingMethod)
        {
            return string.Format("Incorrect accounting method specified: {0}", accountingMethod);
        }
    }
}
