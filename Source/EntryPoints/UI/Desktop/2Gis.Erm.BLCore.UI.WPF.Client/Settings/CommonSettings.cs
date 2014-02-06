using System.Configuration;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Settings
{ 
    // TODO {all, 06.08.2013}: подумать место ли здесь IlocalizationSettings
    public sealed class CommonSettings : 
        ConfigFileSettingsBase, 
        ICommonSettings, 
        IStandartConfigurationSettings, 
        IGlobalizationSettings
    {
        private const string ModuleContainerCommonSettings = "CommonSettings";
        private const string ModuleSettingsSectionPath = SettingsConstants.ModuleContainerSettingsSection + "/" + ModuleContainerCommonSettings;

        public CommonSettings(string configFullPath)
            : base(configFullPath)
        {
        }

        #region Implementation of ICommonSettings

        public bool EnableCaching 
        {
            get
            {
                return ModulesContainerConfig.GetValue<bool>(ModuleSettingsSectionPath, "EnableCaching");
            }
        }

        #endregion

        Configuration IStandartConfigurationSettings.StandartConfiguration
        {
            get
            {
                return Configuration;
            }
        }

        string IGlobalizationSettings.BasicLanguage 
        {
            get { return string.Empty; }
        }

        string IGlobalizationSettings.ReserveLanguage
        {
            get { return string.Empty; }
        }

        BusinessModel IGlobalizationSettings.BusinessModel
        {
            get { return BusinessModel.Russia; }
        }
    }
}