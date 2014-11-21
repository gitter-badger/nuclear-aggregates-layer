using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer
{
    public interface IServiceBusMessageReceiver<TMessageFlow> : IDisposable
        where TMessageFlow : class, IMessageFlow
    {
        IEnumerable<BrokeredMessage> ReceiveBatch(int messageCount);
        void CompleteBatch(IEnumerable<Guid> lockTokens);
    }
}