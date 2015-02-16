namespace DoubleGis.Erm.Platform.DAL
{
    public sealed partial class UnitOfWorkScope
    {
        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
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
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                _pendingChangesHandlingStrategy.HandlePendingChanges(this);

                var modifiableDomainContexts = _unitOfWork.DeattachModifiableDomainContexts(this);
                if (modifiableDomainContexts != null)
                {
                    foreach (var domainContext in modifiableDomainContexts)
                    {
                        domainContext.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }
    }
}