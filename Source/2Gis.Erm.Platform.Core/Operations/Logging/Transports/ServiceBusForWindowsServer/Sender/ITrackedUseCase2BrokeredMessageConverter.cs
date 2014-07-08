using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Sender
{
    public interface ITrackedUseCase2BrokeredMessageConverter
    {
        IEnumerable<BrokeredMessage> Convert(TrackedUseCase trackedUseCase);
    }
}