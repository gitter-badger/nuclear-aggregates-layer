using System;
using System.Configuration;
using System.IO;
using System.Xml;

using nJupiter.Configuration;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings
{
    public abstract class ConfigFileSettingsBase : IDisposable
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

        #region Поддержка IDisposable и Finalize

        private readonly object _disposeSync = new Object();
        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;
        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный + для подклассов
        /// </summary>
        protected bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Внутренний dispose самого базового класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                // сначала вызываем реализацию у потомков
                OnDispose(disposing);
                // теперь отрабатывает сам базовый класс
                if (disposing)
                {
                    // Free other state (managed objects).
                    // TODO
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                var config = _configuration;
                try
                {
                    if (!string.IsNullOrEmpty(config.FilePath) && File.Exists(config.FilePath))
                    {
                        File.Delete(config.FilePath);
                    }
                }
                catch (Exception)
                {
                    // do nothing
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Обработчик dispose для подклассов
        /// </summary>
        protected virtual void OnDispose(bool disposing)
        {
        }


        #endregion
    }
}
