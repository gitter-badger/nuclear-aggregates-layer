using System;

using Microsoft.Practices.Prism.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.MVVM
{
    /// <summary>
    /// Реализация IDisposable базового класса иерархии. Без поддержки Finalize
    /// </summary>
    public class ViewModelBase : NotificationObject, IDisposable
    {
        #region Поддержка IDisposable и Finalize

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
                // TODO

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
