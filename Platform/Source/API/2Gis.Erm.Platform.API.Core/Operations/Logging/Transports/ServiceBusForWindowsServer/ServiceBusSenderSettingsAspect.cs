using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusSenderSettingsAspect : ISettingsAspect, IServiceBusMessageSenderSettings
    {
        private readonly StringSetting _transportEntityPath = ConfigFileSetting.String.Optional("TransportEntityPath", "topic.performedoperations");
        private readonly IntSetting _connectionsCount = ConfigFileSetting.Int.Optional("ServiceBusSenderConnectionsCount", 1);
        private readonly string _connectionString;

        public ServiceBusSenderSettingsAspect(string connectionString)
        {
            _connectionString = connectionString;
        }

        string IServiceBusMessageSenderSettings.TransportEntityPath
        {
            get { return _transportEntityPath.Value; }
        }

        int IServiceBusMessageSenderSettings.ConnectionsCount
        {
            get { return _connectionsCount.Value; }
        }

        string IServiceBusMessageSenderSettings.ConnectionString
        {
            get { return _connectionString; }
        }
    }
}