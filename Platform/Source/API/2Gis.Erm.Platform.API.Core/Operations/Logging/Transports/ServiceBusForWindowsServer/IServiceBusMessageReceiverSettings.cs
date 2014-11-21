using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public interface IServiceBusMessageReceiverSettings : ISettings
    {
        string TransportEntityPath { get; }
        int ConnectionsCount { get; }
        bool UseTransactions { get; }
        string ConnectionString { get; }
    }
}