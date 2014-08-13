using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed partial class ServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly ICommonLog _logger;
        private readonly SenderSlot[] _senderSlots;
        
        public ServiceBusMessageSender(
            IServiceBusMessageSenderSettings serviceBusMessageSenderSettings,
            ICommonLog logger)
        {
            _logger = logger;
            _senderSlots = new SenderSlot[serviceBusMessageSenderSettings.ConnectionsCount];
            
            for (var i = 0; i < _senderSlots.Length; i++)
            {
                var factory = MessagingFactory.CreateFromConnectionString(serviceBusMessageSenderSettings.ConnectionString);
                factory.RetryPolicy = Microsoft.ServiceBus.RetryPolicy.NoRetry;

                var slot = new SenderSlot(logger, factory, serviceBusMessageSenderSettings.TransportEntityPath);
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
            private readonly ICommonLog _logger;
            private readonly MessagingFactory _messagingFactory;
            private readonly string _entityPath;

            private readonly MessageSender _messageSender;
            private readonly RetryPolicy<ServiceBusTransientErrorDetectionStrategy> _retryPolicy =
                new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(3, TimeSpan.FromSeconds(1));

            private int _activeTransmissions;
            
            public SenderSlot(ICommonLog logger, MessagingFactory messagingFactory, string entityPath)
            {
                _retryPolicy.Retrying += OnRetrying;

                _logger = logger;
                _messagingFactory = messagingFactory;
                _entityPath = entityPath;
                _messageSender = _retryPolicy.ExecuteAction(() => messagingFactory.CreateMessageSender(entityPath));
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
                _retryPolicy.ExecuteAction(() => _messageSender.SendBatch(messages));
            }

            public void Dispose()
            {
                _retryPolicy.Retrying -= OnRetrying;
                _messagingFactory.Close();
                _messageSender.Close();
            }

            private void OnRetrying(object sender, RetryingEventArgs args)
            {
                _logger.WarnFormatEx("Retrying to execute action within ServiceBus Message Sender " +
                                     "Current retry count = {0}, last exception: {1}",
                                     args.CurrentRetryCount,
                                     args.LastException.ToString());
            }
        }
    }
}