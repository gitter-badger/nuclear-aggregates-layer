using System;
using System.Configuration;
using System.IO;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public static class ErmEnvironmentsSettingsLoader
    {
        public const string DefaultEnvironmentsConfigFileName = "Environments.config";

        public static string DefaultEnvironmentsConfigFullPath
        {
            get
            {
                var domain = AppDomain.CurrentDomain;
                return Path.Combine(string.IsNullOrEmpty(domain.RelativeSearchPath)
                                        ? domain.BaseDirectory
                                        : domain.RelativeSearchPath,
                                    DefaultEnvironmentsConfigFileName);
            }
        }

        public static ErmEnvironmentsSettingsAspect Load(string environmentsConfigFullPath, string environmentName, string entryPointName)
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap
                    {
                        ExeConfigFilename = environmentsConfigFullPath
                    }, 
                ConfigurationUserLevel.None);
            return new ErmEnvironmentsSettingsAspect(config, environmentName, entryPointName);
        }
    }
}