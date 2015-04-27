using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public sealed class LockingScope : ILockingScope
    {
        private readonly IApplicationLocksReleaser _applicationLocksReleaser;

        private readonly Guid _id;

        private bool _isCompleted;
        private bool _isDisposed;

        public LockingScope(Guid id, IApplicationLocksReleaser applicationLocksReleaser)
        {
            _id = id;
            _applicationLocksReleaser = applicationLocksReleaser;
        }

        public void Complete()
        {
            ThrowIfDisposed();
            _isCompleted = true;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _applicationLocksReleaser.Release(_id, _isCompleted);

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