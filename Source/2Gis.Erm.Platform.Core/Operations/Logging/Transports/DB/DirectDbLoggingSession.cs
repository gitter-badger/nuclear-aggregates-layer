using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class DirectDbLoggingSession : LoggingSession
    {
        public DirectDbLoggingSession() : base(new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
        {
        }
    }
}