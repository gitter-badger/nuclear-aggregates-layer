using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Receiver
{
    public interface IServiceBusMessageReceiver<TMessageFlow> : IDisposable
        where TMessageFlow : class, IMessageFlow
    {
        IEnumerable<BrokeredMessage> ReceiveBatch(int messageCount);
        void CompleteBatch(IEnumerable<Guid> lockTokens);
    }
}