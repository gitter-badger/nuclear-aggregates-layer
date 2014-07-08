using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public sealed class ServiceBusMessageSender : IServiceBusMessageSender
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
            
            for (int i = 0; i < _senderSlots.Length; i++)
            {
                var factory = MessagingFactory.CreateFromConnectionString(_serviceBusMessageSenderSettings.ConnectionString);
                var slot = new SenderSlot
                    {
                        ActiveTransmissions = 0,
                        Factory = factory,
                        Sender = factory.CreateMessageSender(_serviceBusMessageSenderSettings.TransportEntityPath)
                    };

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

            Interlocked.Increment(ref targetSlot.ActiveTransmissions);

            TransactionScope scope = null;

            try
            {
                if (_serviceBusMessageSenderSettings.UseTransactions)
                {
                    scope = new TransactionScope(
                                    TransactionScopeOption.RequiresNew,
                                    new TransactionOptions
                                        {
                                            IsolationLevel = IsolationLevel.Serializable,
                                            Timeout = TimeSpan.Zero
                                        });
                }

                targetSlot.Sender.SendBatch(messages);

                if (scope != null)
                {
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't send data to service bus with entitypath {0}", _serviceBusMessageSenderSettings.TransportEntityPath);
                return false;
            }
            finally
            {
                if (scope != null)
                {
                    scope.Dispose();
                }

                Interlocked.Decrement(ref targetSlot.ActiveTransmissions);
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
            for (int i = 1; i < _senderSlots.Length; i++)
            {
                if (_senderSlots[i].ActiveTransmissions < resolvedSlot.ActiveTransmissions)
                {
                    resolvedSlot = _senderSlots[i];
                }
            }

            return true;
        }

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    foreach (var senderSlot in _senderSlots)
                    {
                        senderSlot.Sender.Close();
                        senderSlot.Factory.Close();
                    }
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }

        #endregion

        private class SenderSlot
        {
            public MessagingFactory Factory { get; set; }
            public MessageSender Sender { get; set; }
            public int ActiveTransmissions;
        }
    }
}