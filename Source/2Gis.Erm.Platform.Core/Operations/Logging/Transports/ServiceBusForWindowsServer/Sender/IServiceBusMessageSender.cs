using System.Collections.Generic;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public interface IServiceBusMessageSender
    {
        bool TrySend(IEnumerable<BrokeredMessage> messages);
    }
}
