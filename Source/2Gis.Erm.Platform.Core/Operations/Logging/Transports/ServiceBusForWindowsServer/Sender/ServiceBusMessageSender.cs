using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed partial class ServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly ICommonLog _logger;
        private readonly ServiceBusConnectionPool<SenderSlot, MessageSender> _serviceBusConnectionPool;
        
        public ServiceBusMessageSender(IServiceBusMessageSenderSettings serviceBusMessageSenderSettings,
                                       ICommonLog logger)
        {
            _logger = logger;

            _serviceBusConnectionPool = new ServiceBusConnectionPool<SenderSlot, MessageSender>(
                serviceBusMessageSenderSettings.ConnectionsCount,
                serviceBusMessageSenderSettings.ConnectionString,
                factory => new SenderSlot(logger, () => factory.CreateMessageSender(serviceBusMessageSenderSettings.TransportEntityPath)));
        }

        public bool TrySend(IEnumerable<BrokeredMessage> messages)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            SenderSlot senderSlot;
            if (!_serviceBusConnectionPool.TryResolveTargetSlot(out senderSlot))
            {
                _logger.DebugEx("Can't resolve target sender slot");
                return false;
            }

            try
            {
                senderSlot.SendBatch(messages);
            }
            catch (Exception ex)
            {
                var entityPath = senderSlot.GetClientPropertyValue(client => client.Path);
                _logger.ErrorFormatEx(ex, "Can't send data to service bus with entitypath {0}", entityPath);
                return false;
            }

            return true;
        }

        private class SenderSlot : ServiceBusConnectionSlot<MessageSender>
        {
            public SenderSlot(ICommonLog logger, Func<MessageSender> messageClientEntityFactory)
                : base(logger, messageClientEntityFactory)
            {
            }

            public void SendBatch(IEnumerable<BrokeredMessage> messages)
            {
                ExecuteAction(messageSender => messageSender.SendBatch(messages));
            }
        }
    }
}