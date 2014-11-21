using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Settings
{ 
    public sealed class CommonSettings : 
        ConfigFileSettingsBase, 
        ICommonSettings, 
        IStandartConfigurationSettings, 
        IGlobalizationSettings
    {
        private const string ModuleContainerCommonSettings = "CommonSettings";
        private const string ModuleSettingsSectionPath = SettingsConstants.ModuleContainerSettingsSection + "/" + ModuleContainerCommonSettings;

        private readonly Type _hardcodedBusinessModelIndicator = typeof(IRussiaAdapted);

        public CommonSettings(string configFullPath)
            : base(configFullPath)
        {
            var globalSpecs = _hardcodedBusinessModelIndicator.GetCustomAttribute<GlobalizationSpecsAttribute>();

            BusinessModel = globalSpecs.BusinessModel;
            BusinessModelIndicator = _hardcodedBusinessModelIndicator;
            DefaultCulture = globalSpecs.CulturesSequence.First();
            ApplicationCulture = DefaultCulture;
            SupportedCultures = new[] { ApplicationCulture };
            SignificantDigitsNumber = 2;
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

        public int SignificantDigitsNumber { get; private set; }
        public BusinessModel BusinessModel { get; private set; }
        public Type BusinessModelIndicator { get; private set; }

        public CultureInfo[] SupportedCultures { get; private set; }
        public CultureInfo DefaultCulture { get; private set; }
        public CultureInfo ApplicationCulture { get; private set; }
    }
}