using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Api.Settings
{
    public class ApiTestModuleSettings : ConfigFileSettingsBase, IApiTestModuleSettings
    {
        private const string ModuleName = "Test";
        
        private readonly string _moduleSettingsRelativePath = string.Format(@"{0}/{1}/{2}", SettingsConstants.ModuleContainerSettingsSection, SettingsConstants.ModulesSettingsPartName, ModuleName);

        #region Implementation of ITestModuleSettings

        public ApiTestModuleSettings(string configFullPath)
            : base(configFullPath)
        {
        }

        public int TestPropertyValue 
        {
            get
            {
                return ModulesContainerConfig.GetValue<int>(_moduleSettingsRelativePath + "/TestPropertyValue");
            }
        }

        #endregion
    }
}