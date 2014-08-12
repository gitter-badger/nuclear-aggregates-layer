using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed partial class ServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly IServiceBusMessageSenderSettings _serviceBusMessageSenderSettings;
        private readonly ICommonLog _logger;
        private readonly SenderSlot[] _senderSlots;
        
        public ServiceBusMessageSender(
            IServiceBusMessageSenderSettings serviceBusMessageSenderSettings,
            ICommonLog logger)
        {
            _serviceBusMessageSenderSettings = serviceBusMessageSenderSettings;
            _logger = logger;
            _senderSlots = new SenderSlot[_serviceBusMessageSenderSettings.ConnectionsCount];
            
            for (var i = 0; i < _senderSlots.Length; i++)
            {
                var factory = MessagingFactory.CreateFromConnectionString(_serviceBusMessageSenderSettings.ConnectionString);
                var slot = new SenderSlot(factory, _serviceBusMessageSenderSettings.TransportEntityPath);

                _senderSlots[i] = slot;
            }
        }

        public bool TrySend(IEnumerable<BrokeredMessage> messages)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            SenderSlot targetSlot;
            if (!TryResolveTargetSlot(out targetSlot))
            {
                _logger.DebugEx("Can't resolve target sender slot");
                return false;
            }

            targetSlot.IncrementActiveTransmissions();

            try
            {
                targetSlot.SendBatch(messages);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't send data to service bus with entitypath {0}", targetSlot.EntityPath);
                return false;
            }
            finally
            {
                targetSlot.DecrementActiveTransmissions();
            }

            return true;
        }

        private bool TryResolveTargetSlot(out SenderSlot resolvedSlot)
        {
            resolvedSlot = null;

            if (!_senderSlots.Any())
            {
                return false;
            }

            resolvedSlot = _senderSlots[0];
            for (var i = 1; i < _senderSlots.Length; i++)
            {
                if (_senderSlots[i].ActiveTransmissions < resolvedSlot.ActiveTransmissions)
                {
                    resolvedSlot = _senderSlots[i];
                }
            }

            return true;
        }

        private class SenderSlot : IDisposable
        {
            private readonly string _entityPath;
            private readonly MessagingFactory _messagingFactory;
            private readonly MessageSender _messageSender;

            private int _activeTransmissions;
            
            public SenderSlot(MessagingFactory messagingFactory, string entityPath)
            {
                _messagingFactory = messagingFactory;
                _entityPath = entityPath;
                _messageSender = messagingFactory.CreateMessageSender(entityPath);
            }

            public string EntityPath
            {
                get { return _entityPath; }
            }

            public int ActiveTransmissions
            {
                get { return _activeTransmissions; }
            }

            public void IncrementActiveTransmissions()
            {
                Interlocked.Increment(ref _activeTransmissions);
            }

            public void DecrementActiveTransmissions()
            {
                Interlocked.Decrement(ref _activeTransmissions);
            }

            public void SendBatch(IEnumerable<BrokeredMessage> messages)
            {
                _messageSender.SendBatch(messages);
            }

            public void Dispose()
            {
                _messagingFactory.Close();
                _messageSender.Close();
            }
        }
    }
}