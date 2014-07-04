using System;

namespace DoubleGis.Erm.Platform.DI.Proxies
{
    public abstract partial class UnityContainerScopeProxy<TProxiedContract>
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
                    var disposedProxiedInstance = ProxiedInstance as IDisposable;
                    if (disposedProxiedInstance != null)
                    {
                        disposedProxiedInstance.Dispose();
                    }

                    _unityContainer.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }
    }
}