using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings
{
    public sealed class ServiceBusSenderSettingsAspect : ISettingsAspect, IServiceBusMessageSenderSettings
    {
        private readonly StringSetting _transportEntityPath = ConfigFileSetting.String.Optional("TransportEntityPath", "queue-single");
        private readonly IntSetting _connectionsCount = ConfigFileSetting.Int.Optional("ConnectionsCount", 1);
        private readonly BoolSetting _useTransactions = ConfigFileSetting.Bool.Optional("UseTransactions", true);
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

        bool IServiceBusMessageSenderSettings.UseTransactions
        {
            get { return _useTransactions.Value; }
        }

        string IServiceBusMessageSenderSettings.ConnectionString
        {
            get { return _connectionString; }
        }
    }
}