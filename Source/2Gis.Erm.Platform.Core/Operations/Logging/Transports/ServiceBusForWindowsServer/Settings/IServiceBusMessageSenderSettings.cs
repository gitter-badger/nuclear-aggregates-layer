using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings
{
    public interface IServiceBusMessageSenderSettings : ISettings
    {
        string TransportEntityPath { get; }
        int ConnectionsCount { get; }
        bool UseTransactions { get; }
        string ConnectionString { get; }
    }
}