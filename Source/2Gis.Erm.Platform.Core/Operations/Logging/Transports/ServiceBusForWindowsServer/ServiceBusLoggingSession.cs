using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusLoggingSession : LoggingSession
    {
        public ServiceBusLoggingSession()
            : base(new TransactionScope(TransactionScopeOption.RequiresNew,
                                        new TransactionOptions
                                            {
                                                // ограничение API service bus поддерживается только уровень изоляции Serializable, 
                                                // поэтому невозможно присоединиться к ambient transaction (который в ERM обычно имеет уровень изоляции snapshot)
                                                IsolationLevel = IsolationLevel.Serializable,
                                                Timeout = DefaultTransactionOptions.Default.Timeout
                                            }))
        {
        }
    }
}