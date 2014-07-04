using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusOperationsReceiver<TMessageFlow> :
        MessageReceiverBase<TMessageFlow, ServiceBusPerformedOperationsMessage, IPerformedOperationsReceiverSettings> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly ICommonLog _logger;
        private readonly QueueClient _queueClient;

        public ServiceBusOperationsReceiver(
            IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings,
            IPerformedOperationsReceiverSettings messageReceiverSettings, 
            ICommonLog logger) 
            : base(messageReceiverSettings)
        {
            _logger = logger;

            MessagingFactory messagingFactory = MessagingFactory.CreateFromConnectionString(serviceBusMessageReceiverSettings.ConnectionString);
            _queueClient = messagingFactory.CreateQueueClient(serviceBusMessageReceiverSettings.TransportEntityPath, ReceiveMode.PeekLock);
        }

        protected override IEnumerable<ServiceBusPerformedOperationsMessage> Peek()
        {
            try
            {
                var receivedMessage = _queueClient.Receive(TimeSpan.FromMinutes(1));
                return new[] { new ServiceBusPerformedOperationsMessage(new[] { receivedMessage }) };
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Can't receive messages from service bus for message flow " + SourceMessageFlow);
                throw;
            }
        }

        protected override void Complete(
            IEnumerable<ServiceBusPerformedOperationsMessage> successfullyProcessedMessages, 
            IEnumerable<ServiceBusPerformedOperationsMessage> failedProcessedMessages)
        {
            try
            {
                foreach (var successfullyProcessedMessage in successfullyProcessedMessages)
                {
                    foreach (var operationMsg in successfullyProcessedMessage.Operations)
                    {
                        operationMsg.Complete();
                    }
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
            var queueClient = _queueClient;
            if (queueClient != null)
            {
                queueClient.Close();
            }
        }
    }
}
