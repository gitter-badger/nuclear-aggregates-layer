using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.ServiceBus.Messaging;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class ServiceBusConnectionPool<TSlot, TClient> : IDisposable
        where TSlot : ServiceBusConnectionSlot<TClient>
        where TClient : MessageClientEntity
    {
        private readonly MessagingFactory _messagingFactory;
        private readonly TSlot[] _connectionSlots;
        public ServiceBusConnectionPool(int capacity, string connectionString, Func<MessagingFactory, TSlot> connectionSlotFactory)
        {
            _messagingFactory = MessagingFactory.CreateFromConnectionString(connectionString);
            _messagingFactory.RetryPolicy = Microsoft.ServiceBus.RetryPolicy.NoRetry;
            
            _connectionSlots = new TSlot[capacity];
            for (var i = 0; i < _connectionSlots.Length; i++)
            {
                _connectionSlots[i] = connectionSlotFactory(_messagingFactory);
            }
        }

        public bool TryResolveTargetSlot(out TSlot resolvedSlot)
        {
            resolvedSlot = null;

            if (!_connectionSlots.Any())
            {
                return false;
            }

            resolvedSlot = _connectionSlots[0];
            for (var i = 1; i < _connectionSlots.Length; i++)
            {
                if (_connectionSlots[i].ActiveTransmissions < resolvedSlot.ActiveTransmissions)
                {
                    resolvedSlot = _connectionSlots[i];
                }
            }

            return true;
        }

        public void Dispose()
        {
            _messagingFactory.Close();
            foreach (var slot in _connectionSlots)
            {
                slot.Dispose();
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal abstract class ServiceBusConnectionSlot<TClient> : IDisposable where TClient : MessageClientEntity
    {
        private readonly ITracer _tracer;
        private readonly TClient _clientEntity;
        private readonly RetryPolicy<ServiceBusTransientErrorDetectionStrategy> _retryPolicy =
                new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(3, TimeSpan.FromSeconds(1));

        private int _activeTransmissions;

        protected ServiceBusConnectionSlot(ITracer tracer, Func<TClient> messageClientEntityFactory)
        {
            _tracer = tracer;
            _clientEntity = messageClientEntityFactory();

            _retryPolicy.Retrying += OnRetrying;
        }

        public int ActiveTransmissions
        {
            get { return _activeTransmissions; }
        }

        public TProperty GetClientPropertyValue<TProperty>(Func<TClient, TProperty> func)
        {
            return func(_clientEntity);
        }

        public void ExecuteAction(Action<TClient> action)
        {
            _retryPolicy.ExecuteAction(() =>
                {
                    Interlocked.Increment(ref _activeTransmissions);
                    try
                    {
                        action(_clientEntity);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _activeTransmissions);
                    }
                });
        }

        public TResult ExecuteAction<TResult>(Func<TClient, TResult> func)
        {
            return _retryPolicy.ExecuteAction(() =>
                {
                    Interlocked.Increment(ref _activeTransmissions);
                    try
                    {
                        return func(_clientEntity);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _activeTransmissions);
                    }
                });
        }

        public void Dispose()
        {
            _retryPolicy.Retrying -= OnRetrying;

            // COMMENT {all, 13.08.2014}: an additional layer of protection in the form of a try/catch construct is recommended
            // http://msdn.microsoft.com/en-us/library/hh851740.aspx
            try
            {
                _clientEntity.Close();
            }
            catch (Exception ex)
            {
                _tracer.WarnFormat("Error occured while closing Service Bus messaging objects. Exception: {0}", ex.ToString());
            }
        }

        private void OnRetrying(object sender, RetryingEventArgs args)
        {
            _tracer.WarnFormat("Retrying to execute action within ServiceBus connection slot." +
                                 "Current retry count = {0}, last exception: {1}",
                                 args.CurrentRetryCount,
                                 args.LastException.ToString());
        }
    }
}