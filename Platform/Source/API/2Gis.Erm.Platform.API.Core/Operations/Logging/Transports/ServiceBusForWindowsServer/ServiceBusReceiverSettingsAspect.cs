using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusReceiverSettingsAspect : ISettingsAspect, IServiceBusMessageReceiverSettings
    {
        private readonly StringSetting _transportEntityPath = ConfigFileSetting.String.Optional("TransportEntityPath", "topic.performedoperations");
        private readonly IntSetting _connectionsCount = ConfigFileSetting.Int.Optional("ConnectionsCount", 1);
        private readonly BoolSetting _useTransactions = ConfigFileSetting.Bool.Optional("UseTransactions", true);
        private readonly string _connectionString;

        public ServiceBusReceiverSettingsAspect(string connectionString)
        {
            _connectionString = connectionString;
        }

        int IServiceBusMessageReceiverSettings.ConnectionsCount
        {
            get { return _connectionsCount.Value; }
        }

        bool IServiceBusMessageReceiverSettings.UseTransactions
        {
            get { return _useTransactions.Value; }
        }

        string IServiceBusMessageReceiverSettings.ConnectionString
        {
            get { return _connectionString; }
        }

        string IServiceBusMessageReceiverSettings.TransportEntityPath
        {
            get { return _transportEntityPath.Value; }
        }
    }
}