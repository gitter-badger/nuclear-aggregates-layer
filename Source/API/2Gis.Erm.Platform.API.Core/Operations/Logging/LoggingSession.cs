using System;
using System.Transactions;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public abstract class LoggingSession : IDisposable
    {
        private readonly TransactionScope _transactionScope;

        protected LoggingSession(TransactionOptions transactionOptions)
        {
            _transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions);
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