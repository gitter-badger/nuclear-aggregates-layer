using System;
using System.Configuration;
using System.IO;
using System.Xml;

using nJupiter.Configuration;

using Nuclear.Settings.API;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings
{
    public abstract partial class ConfigFileSettingsBase : SettingsContainerBase
    {
        private readonly IConfig _configSource;
        private readonly Configuration _configuration;

        protected ConfigFileSettingsBase(string configFileFullPath)
        {
            if (string.IsNullOrEmpty(configFileFullPath) || !File.Exists(configFileFullPath))
            {
                throw new ArgumentException(string.Format("Specified config file full path {0} is not correct path for existing file", configFileFullPath));
            }

            _configSource = FileConfigFactory.CreateWithWatcher(configFileFullPath);
            _configuration = LoadStandartConfiguration(configFileFullPath);
        }
        
        protected IConfig ModulesContainerConfig
        {
            get
            {
                return _configSource;
            }
        }

        protected Configuration Configuration
        {
            get
            {
                return _configuration;
            }
        }
        
        private static string GetTempFileFullPath(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + "." + extension;
            return Path.Combine(path, fileName);
        }

        private Configuration LoadStandartConfiguration(string configFileFullPath)
        {
            // общий прицип из конфиг файла соответствующего конкретному контейнеру модулей
            // выделяем часть стандартного .NET config
            var tempFileFullPath = GetTempFileFullPath("config");
            using (var writer = XmlWriter.Create(tempFileFullPath))
            {
                _configSource.ConfigXml.WriteTo(writer);
            }
           
            // удаляем из документа часть относящуюся к настройкам контенейров модулей/модулей
            // цель - оставить внутри тэга configuration только корректное содержимое стандартного config файла .NET
            var document = new XmlDocument();
            document.Load(tempFileFullPath);

            var byModulesPart = document.SelectSingleNode("configuration/ModuleContainerSettings");
            if (byModulesPart == null || byModulesPart.ParentNode == null)
            {
                throw new InvalidOperationException("Invalid config file format. File path: " + configFileFullPath);
            }

            byModulesPart.ParentNode.RemoveChild(byModulesPart);
            document.Save(tempFileFullPath);

            // загружаем стандартный .NET config из временного файла
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = tempFileFullPath }, ConfigurationUserLevel.None);
            
            return configuration;
        }
    }
}
