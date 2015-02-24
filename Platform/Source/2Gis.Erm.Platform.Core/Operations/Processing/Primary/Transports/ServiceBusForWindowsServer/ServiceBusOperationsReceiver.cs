using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, ServiceBusPerformedOperationsMessage, IPerformedOperationsReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly ICommonLog _logger;
        private readonly IServiceBusMessageReceiver<TMessageFlow> _serviceBusMessageReceiver;

        public ServiceBusOperationsReceiver(
            IPerformedOperationsReceiverSettings messageReceiverSettings, 
            IServiceBusMessageReceiver<TMessageFlow> serviceBusMessageReceiver,
            ICommonLog logger)
            : base(messageReceiverSettings)
        {
            _logger = logger;
            _serviceBusMessageReceiver = serviceBusMessageReceiver;
        }

        protected override IReadOnlyList<ServiceBusPerformedOperationsMessage> Peek()
        {
            var batch = _serviceBusMessageReceiver.ReceiveBatch(MessageReceiverSettings.BatchSize);
            return batch.Select(brokeredMessage => new ServiceBusPerformedOperationsMessage(new[] { brokeredMessage })).ToList();
        }

        protected override void Complete(
            IEnumerable<ServiceBusPerformedOperationsMessage> successfullyProcessedMessages,
            IEnumerable<ServiceBusPerformedOperationsMessage> failedProcessedMessages)
        {
            if (successfullyProcessedMessages.Any())
            {
                var lockTokens = successfullyProcessedMessages.SelectMany(m => m.Operations).Select(brokeredMessage => brokeredMessage.LockToken);
                _serviceBusMessageReceiver.CompleteBatch(lockTokens);
            }

            foreach (var failedProcessedMessage in failedProcessedMessages)
            {
                foreach (var brokeredMessage in failedProcessedMessage.Operations)
                {
                    try
                    {
                        brokeredMessage.Abandon();
                    }
                    catch (Exception ex)
                    {
                                _logger.ErrorFormat(ex, "Service Bus message with Id={0} cannot be abandoned", brokeredMessage.MessageId);
                        throw;
                    }
                }
            }
        }
    }
}
