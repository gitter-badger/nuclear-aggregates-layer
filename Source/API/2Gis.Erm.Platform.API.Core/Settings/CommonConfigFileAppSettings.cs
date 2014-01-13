using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    /// <summary>
    /// Базовый класс для получения настроек приложения из *.config файла точки входа. 
    /// Содержит общиий функционал и общие настройки для всех точек входа в приложение.
    /// </summary>
    public abstract class CommonConfigFileAppSettings : IAppSettings
    {
        private const int WarmClientDaysCountDefault = 14;

        private readonly StringSetting _basicLanguage = ConfigFileSetting.String.Required("BasicLanguage");
        private readonly StringSetting _reserveLanguage = ConfigFileSetting.String.Required("ReserveLanguage");
        private readonly EnumSetting<BusinessModel> _businessModel = ConfigFileSetting.Enum.Required<BusinessModel>("BusinessModel");
        private readonly BoolSetting _enableCaching = ConfigFileSetting.Bool.Required("EnableCaching");
        private readonly BoolSetting _enableNotifications = ConfigFileSetting.Bool.Required("EnableNotifications");
        private readonly DecimalSetting _minDebtAmount = ConfigFileSetting.Decimal.Required("MinDebtAmount");
        private readonly IntSetting _orderRequestProcessingHoursAmount = ConfigFileSetting.Int.Required("OrderRequestProcessingHoursAmount");
        private readonly StringSetting _reserveUserAccount = ConfigFileSetting.String.Required("ReserveUserAccount");
        private readonly IntSetting _significantDigitsNumber = ConfigFileSetting.Int.Required("SignificantDigitsNumber");
        private readonly IntSetting _warmClientDaysCount = ConfigFileSetting.Int.Optional("WarmClientDaysCount", WarmClientDaysCountDefault);
        
        private readonly EnumSetting<AppTargetEnvironment> _targetEnvironment = ConfigFileSetting.Enum.Required<AppTargetEnvironment>("TargetEnvironment");
        private readonly StringSetting _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName");
        private readonly StringSetting _entryPointName = ConfigFileSetting.String.Required("EntryPointName");

        private readonly ConnectionStringsSettingsAspect _connectionStrings;

        protected readonly MsCRMSettingsAspect MsCRMSettings;
        protected readonly APIServicesSettingsAspect APIServicesSettings;

        protected CommonConfigFileAppSettings()
        {
            var ermEnvironmentSettings = ErmEnvironmentsSettingsLoader.Load(ErmEnvironmentsSettingsLoader.DefaultEnvironmentsConfigFullPath,
                                                                            _targetEnvironmentName.Value,
                                                                            _entryPointName.Value);

            _connectionStrings = new ConnectionStringsSettingsAspect(ermEnvironmentSettings.ConnectionStrings);
            MsCRMSettings = new MsCRMSettingsAspect(_connectionStrings);
            APIServicesSettings = new APIServicesSettingsAspect(ermEnvironmentSettings.AvailableServices);
        }

        public string ReserveUserAccount
        {
            get { return _reserveUserAccount.Value; }
        }

        public bool EnableNotifications
        {
            get { return _enableNotifications.Value; }
        }

        public bool EnableCaching
        {
            get { return _enableCaching.Value; }
        }

        public int WarmClientDaysCount
        {
            get { return _warmClientDaysCount.Value; }
            }

        public int SignificantDigitsNumber
        {
            get { return _significantDigitsNumber.Value; }
        }

        public decimal MinDebtAmount
        {
            get { return _minDebtAmount.Value; }
        }

        public int OrderRequestProcessingHoursAmount
        {
            get
            {
                return _orderRequestProcessingHoursAmount.Value;
            }
        }
        
        public AppTargetEnvironment TargetEnvironment
        {
            get { return _targetEnvironment.Value; }
        }

        public string EntryPointName
        {
            get { return _entryPointName.Value; }
        }

        public string BasicLanguage
        {
            get { return _basicLanguage.Value; }
        }

        public string ReserveLanguage
        {
            get { return _reserveLanguage.Value; }
        }

        public BusinessModel BusinessModel
        {
            get { return _businessModel.Value; }
        }

        public ConnectionStringsSettingsAspect ConnectionStrings 
        {
            get { return _connectionStrings; }
        }

        public string TargetEnvironmentName
        {
            get { return _targetEnvironmentName.Value; }
        }
    }
}