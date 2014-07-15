using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings
{
    public sealed class ServiceBusReceiverSettingsAspect : ISettingsAspect, IServiceBusMessageReceiverSettings
    {
        private readonly StringSetting _transportEntityPath = ConfigFileSetting.String.Optional("TransportEntityPath", "performedoperations.topic.main");
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