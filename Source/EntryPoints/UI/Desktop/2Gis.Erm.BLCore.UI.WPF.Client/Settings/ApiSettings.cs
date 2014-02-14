using System;
using System.IO;

using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Settings
{
    public class ApiSettings : ConfigFileSettingsBase, IApiSettings 
    {
        private const string ModuleContainerApiSettings = "ApiSettings";
        private const string ModuleSettingsSectionPath = SettingsConstants.ModuleContainerSettingsSection + "/" + ModuleContainerApiSettings;

        private readonly APIServicesSettingsAspect _APIServicesSettings;
        
        public ApiSettings(string configFileFullPath) : base(configFileFullPath)
        {
            _APIServicesSettings = new APIServicesSettingsAspect();
        }

        public AppTargetEnvironment TargetEnvironment
        {
            get
            {
                var rawValue = ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "TargetEnvironment");
                AppTargetEnvironment buffer;
                if (Enum.TryParse(rawValue, out buffer))
                {
                    return buffer;
                }

                throw new ApplicationException("Can't find required settings TargetEnvironment");
            }
        }

        public string TargetEnvironmentName
        {
            get
            {
                return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "TargetEnvironmentName");
            }
        }

        public string EntryPointName
        {
            get
            {
                return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "EntryPointName");
            }
        }

        public string ApiUrl
        {
            get
            {
                return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "ApiUrl");
            }
        }

        public string ListServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "ListServiceEndpointName"); }
        }

        public string CreateOrUpdateServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "CreateOrUpdateServiceEndpointName"); }
        }

        public string GetDomainEntityDtoServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "GetDomainEntityDtoServiceEndpointName"); }
        }

        public string MetadataServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "MetadataServiceEndpointName"); }
        }

        public string AssignServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "AssignServiceEndpointName"); }
        }

        public string GroupAssignServiceEndpointName
        {
            get { return ModulesContainerConfig.GetValue<string>(ModuleSettingsSectionPath, "GroupAssignServiceEndpointName"); }
        }

        public APIServicesSettingsAspect ServicesSettings 
        {
            get { return _APIServicesSettings; }
        }
    }
}