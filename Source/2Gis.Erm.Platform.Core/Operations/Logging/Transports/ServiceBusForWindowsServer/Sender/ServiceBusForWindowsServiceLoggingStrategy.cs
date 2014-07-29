using System;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed class ServiceBusForWindowsServiceLoggingStrategy : IOperationLoggingStrategy
    {
        private readonly ITrackedUseCase2BrokeredMessageConverter _trackedUseCase2BrokeredMessageConverter;
        private readonly IServiceBusMessageSender _serviceBusMessageSender;
        private readonly ICommonLog _logger;

        public ServiceBusForWindowsServiceLoggingStrategy(
            ITrackedUseCase2BrokeredMessageConverter trackedUseCase2BrokeredMessageConverter,
            IServiceBusMessageSender serviceBusMessageSender,
            ICommonLog logger)
        {
            _trackedUseCase2BrokeredMessageConverter = trackedUseCase2BrokeredMessageConverter;
            _serviceBusMessageSender = serviceBusMessageSender;
            _logger = logger;
        }

        public LoggingSession Begin()
        {
            return new LoggingSession
            {
                Transaction = 
                    new TransactionScope(
                            TransactionScopeOption.RequiresNew, 
                            new TransactionOptions
                                {
                                    IsolationLevel = IsolationLevel.Serializable, // ограничение API service bus поддерживается только уровень изоляции Serializable, поэтому невозможно присоединиться к ambient transaction (который в ERM обычно имеет уровень изоляции snapshot)
                                    Timeout = DefaultTransactionOptions.Default.Timeout
                                })
            };
        }

        public bool TryLogUseCase(TrackedUseCase useCase, out string report)
        {
            report = null;

            try
            {
                var messages = _trackedUseCase2BrokeredMessageConverter.Convert(useCase);
                if (!_serviceBusMessageSender.TrySend(messages))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                report = ex.ToDecription();
                _logger.ErrorFormatEx(ex, "Can't log info about usecase to service bus. Use case details: {0}", useCase);
                
                return false;
            }

            return true;
        }

        public void Complete(LoggingSession loggingSession)
        {
            loggingSession.Transaction.Complete();
        }

        public void Close(LoggingSession loggingSession)
        {
            loggingSession.Transaction.Dispose();
        }
    }
}
