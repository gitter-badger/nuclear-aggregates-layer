using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public interface IServiceBusMessageSenderSettings : ISettings
    {
        string TransportEntityPath { get; }
        int ConnectionsCount { get; }
        string ConnectionString { get; }
    }
}