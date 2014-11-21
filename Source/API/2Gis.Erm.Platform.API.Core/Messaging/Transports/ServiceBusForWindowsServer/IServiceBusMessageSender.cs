using System.Collections.Generic;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer
{
    public interface IServiceBusMessageSender
    {
        bool TrySend(IEnumerable<BrokeredMessage> messages);
    }
}
