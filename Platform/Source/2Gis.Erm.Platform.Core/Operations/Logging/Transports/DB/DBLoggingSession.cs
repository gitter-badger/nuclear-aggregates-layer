using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB
{
    public sealed class DBLoggingSession : LoggingSession
    {
        public DBLoggingSession() : base(new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
        {
        }
    }
}