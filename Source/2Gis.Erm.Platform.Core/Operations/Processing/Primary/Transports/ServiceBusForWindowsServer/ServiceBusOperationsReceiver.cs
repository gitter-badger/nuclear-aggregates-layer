using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, ServiceBusPerformedOperationsMessage, IPerformedOperationsReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly ICommonLog _logger;
        private readonly SubscriptionClient _messageReceiver;

        public ServiceBusOperationsReceiver(
            IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings,
            IPerformedOperationsReceiverSettings messageReceiverSettings, 
            ICommonLog logger) 
            : base(messageReceiverSettings)
        {
            _logger = logger;

            var messagingFactory = MessagingFactory.CreateFromConnectionString(serviceBusMessageReceiverSettings.ConnectionString);
            _messageReceiver = messagingFactory.CreateSubscriptionClient(serviceBusMessageReceiverSettings.TransportEntityPath,
                                                                         SourceMessageFlow.Id.ToString(),
                                                                         ReceiveMode.PeekLock);
        }

        protected override IEnumerable<ServiceBusPerformedOperationsMessage> Peek()
        {
            try
            {
                var receivedMessage = _messageReceiver.ReceiveBatch(MessageReceiverSettings.BatchSize);
                return receivedMessage.Select(bm => new ServiceBusPerformedOperationsMessage(new[] { bm }));
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Can't receive messages from service bus for message flow " + SourceMessageFlow);
                throw;
            }
        }

        protected override void Complete(IEnumerable<ServiceBusPerformedOperationsMessage> successfullyProcessedMessages,
                                         IEnumerable<ServiceBusPerformedOperationsMessage> failedProcessedMessages)
        {
            try
            {
                if (successfullyProcessedMessages.Any())
                {
                    var lockTokens = successfullyProcessedMessages.SelectMany(m => m.Operations).Select(bm => bm.LockToken);
                    _messageReceiver.CompleteBatch(lockTokens);
                }

                foreach (var failedProcessedMessage in failedProcessedMessages)
                {
                    foreach (var operationMsg in failedProcessedMessage.Operations)
                    {
                        operationMsg.Abandon();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Can't complete messages receiving from service bus for message flow " + SourceMessageFlow);
                throw;
            }
        }

        protected override void OnDispose(bool disposing)
        {
            var messageReceiver = _messageReceiver;
            if (messageReceiver != null)
            {
                messageReceiver.Close();
            }
        }
    }
}
