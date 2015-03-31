using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;

using Microsoft.ServiceBus.Messaging;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer
{
    public sealed partial class ServiceBusMessageReceiver<TMessageFlow> : IServiceBusMessageReceiver<TMessageFlow> 
        where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly ITracer _tracer;
        private readonly ServiceBusConnectionPool<ReceiverSlot, SubscriptionClient> _serviceBusConnectionPool;

        public ServiceBusMessageReceiver(ITracer tracer, IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings)
        {
            _tracer = tracer;
            _serviceBusConnectionPool = new ServiceBusConnectionPool<ReceiverSlot, SubscriptionClient>(
                serviceBusMessageReceiverSettings.ConnectionsCount,
                serviceBusMessageReceiverSettings.ConnectionString,
                factory => new ReceiverSlot(tracer, () => CreateSubscriptionClient(factory, serviceBusMessageReceiverSettings.TransportEntityPath)));
        }

        public IEnumerable<BrokeredMessage> ReceiveBatch(int messageCount)
        {
            var receiverSlot = ResolveReceiverSlot();
            try
            {
                return receiverSlot.ReceiveBatch(messageCount);
            }
            catch (Exception ex)
            {
                var topicPath = receiverSlot.GetClientPropertyValue(x => x.TopicPath);
                var flowName = receiverSlot.GetClientPropertyValue(x => x.Name);
                _tracer.ErrorFormat(ex, "Can't receive messages from service bus for message flow {0}/{1}", topicPath, flowName);
                
                throw;
            }
        }

        public void CompleteBatch(IEnumerable<Guid> lockTokens)
        {
            var receiverSlot = ResolveReceiverSlot();
            try
            {
                receiverSlot.CompleteBatch(lockTokens);
            }
            catch (Exception ex)
            {
                var topicPath = receiverSlot.GetClientPropertyValue(x => x.TopicPath);
                var flowName = receiverSlot.GetClientPropertyValue(x => x.Name);
                _tracer.ErrorFormat(ex, "Can't complete messages receiving from service bus for message flow {0}/{1}", topicPath, flowName);
                
                throw;
            }
        }

        private static SubscriptionClient CreateSubscriptionClient(MessagingFactory messagingFactory, string entityPath)
        {
            var messageFlow = new TMessageFlow();
            return messagingFactory.CreateSubscriptionClient(entityPath, messageFlow.Id.ToString(), ReceiveMode.PeekLock);
        }

        private ReceiverSlot ResolveReceiverSlot()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            ReceiverSlot receiverSlot;
            if (!_serviceBusConnectionPool.TryResolveTargetSlot(out receiverSlot))
            {
                throw new ApplicationException("Can't resolve target receiver slot");
            }

            return receiverSlot;
        }

        private class ReceiverSlot : ServiceBusConnectionSlot<SubscriptionClient>
        {
            public ReceiverSlot(ITracer tracer, Func<SubscriptionClient> messageClientEntityFactory)
                : base(tracer, messageClientEntityFactory)
            {
            }

            public IEnumerable<BrokeredMessage> ReceiveBatch(int messageCount)
            {
                return ExecuteAction(subscriptionClient => subscriptionClient.ReceiveBatch(messageCount));
            }

            public void CompleteBatch(IEnumerable<Guid> lockTokens)
            {
                ExecuteAction(subscriptionClient => subscriptionClient.CompleteBatch(lockTokens));
            }
        }
    }
}