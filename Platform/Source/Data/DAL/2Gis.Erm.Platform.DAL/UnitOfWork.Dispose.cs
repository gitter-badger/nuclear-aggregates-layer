namespace DoubleGis.Erm.Platform.DAL
{
    public abstract partial class UnitOfWork
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
        /// Внутренний dispose самого базового класса
        /// </summary>
        public void Dispose()
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                // сначала вызываем реализацию у потомков
                OnDispose();

                _scopedDomainContextsStore.Dispose();
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Обработчик dispose для подклассов
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}