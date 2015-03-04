using System;

using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Common.Utils;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusLoggingStrategy : IOperationLoggingStrategy
    {
        private readonly ITrackedUseCase2BrokeredMessageConverter _trackedUseCase2BrokeredMessageConverter;
        private readonly IServiceBusMessageSender _serviceBusMessageSender;
        private readonly ICommonLog _logger;

        public ServiceBusLoggingStrategy(
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
            return new ServiceBusLoggingSession();
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
                _logger.ErrorFormat(ex, "Can't log info about usecase to service bus. Use case details: {0}", useCase);
                
                return false;
            }

            return true;
        }

        public void Complete(LoggingSession loggingSession)
        {
            loggingSession.Complete();
        }

        public void Close(LoggingSession loggingSession)
        {
            loggingSession.Dispose();
        }
    }
}
