using System.Transactions;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class LoggingSession
    {
        public TransactionScope Transaction { get; set; }
    }
}