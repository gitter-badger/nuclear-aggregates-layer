using System.Collections.Generic;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public interface ITrackedUseCase2BrokeredMessageConverter
    {
        IEnumerable<BrokeredMessage> Convert(TrackedUseCase useCase);
    }
}