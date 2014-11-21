using System;
using System.IO;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings
{
    public abstract partial class ConfigFileSettingsBase : IDisposable
    {
        private readonly object _disposeSync = new object();

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
        /// Обработчик dispose для подклассов
        /// </summary>
        protected virtual void OnDispose(bool disposing)
        {
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
    }
}
