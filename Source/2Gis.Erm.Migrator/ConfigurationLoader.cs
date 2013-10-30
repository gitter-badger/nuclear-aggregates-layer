using System;
using System.Configuration;
using System.IO;

namespace DoubleGis.Erm.Migrator
{
    public static class ConfigurationLoader
    {
        public static Configuration LoadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new ArgumentException("Specified configuration file path does not exist", "path");
            }

            string configFile = path.Trim();

            if (!configFile.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                configFile += ".config";
            }

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };

            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }
    }
}
