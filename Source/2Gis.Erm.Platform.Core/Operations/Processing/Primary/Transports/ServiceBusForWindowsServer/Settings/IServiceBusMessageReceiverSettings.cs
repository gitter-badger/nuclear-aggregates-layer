using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings
{
    public interface IServiceBusMessageReceiverSettings : ISettings
    {
        string TransportEntityPath { get; }
        int ConnectionsCount { get; }
        bool UseTransactions { get; }
        string ConnectionString { get; }
    }
}