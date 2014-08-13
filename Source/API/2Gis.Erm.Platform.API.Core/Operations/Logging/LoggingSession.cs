using System;
using System.Transactions;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public abstract class LoggingSession : IDisposable
    {
        private readonly TransactionScope _transactionScope;

        protected LoggingSession(TransactionScope transactionScope)
        {
            _transactionScope = transactionScope;
        }

        public void Complete()
        {
            _transactionScope.Complete();
        }

        public void Dispose()
        {
            _transactionScope.Dispose();
        }
    }
}