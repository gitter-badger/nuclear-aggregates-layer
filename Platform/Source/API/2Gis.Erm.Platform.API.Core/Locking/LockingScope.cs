using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public sealed class LockingScope : ILockingScope, ITrackedLockingScope
    {
        private readonly Guid _id;
        private bool _isReleased;
        private bool _isDisposed;
        private readonly IApplicationLocksManager _applicationLocksManager;

        public LockingScope(Guid id, IApplicationLocksManager applicationLocksManager)
        {
            _id = id;
            _applicationLocksManager = applicationLocksManager;
        }

        public Guid Id
        {
            get
            {
                ThrowIfDisposed();
                return _id;
            }
        }

        public void Release()
        {
            ThrowIfDisposed();
            if (_isReleased)
            {
                return;
            }

            _applicationLocksManager.Release(this);
            _isReleased = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LockingScope()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing && !_isReleased)
            {
                Release();
            }

            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}