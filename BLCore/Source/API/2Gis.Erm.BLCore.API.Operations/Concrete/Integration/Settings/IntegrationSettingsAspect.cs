using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings
{
    public sealed class IntegrationSettingsAspect : ISettingsAspect, IIntegrationSettings
    {
        private readonly BoolSetting _enableIntegration = ConfigFileSetting.Bool.Required("EnableIntegration");
        private readonly StringSetting _integrationApplicationName = ConfigFileSetting.String.Required("IntegrationApplicationName");
        private readonly BoolSetting _useWarehouseIntegration = ConfigFileSetting.Bool.Required("UseWarehouseIntegration");
        private readonly BoolSetting _enableRabbitMqQueue = ConfigFileSetting.Bool.Required("EnableRabbitMqQueue");

        public bool EnableIntegration
        {
            get
            {
                return _enableIntegration.Value;
            }
        }

        public bool UseWarehouseIntegration
        {
            get
            {
                return _useWarehouseIntegration.Value;
            }
        }

        public string IntegrationApplicationName
        {
            get
            {
                return _integrationApplicationName.Value;
            }
        }

        public bool EnableRabbitMqQueue
        {
            get
            {
                return _enableRabbitMqQueue.Value;
            }
        }
    }
}