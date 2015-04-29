using System;

using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.Common.Identities;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class AppLockIdentityServiceUniqueIdProvider : IIdentityServiceUniqueIdProvider, IDisposable
    {
        private const string IdAppLockTemplate = "Id={0}";

        private readonly IApplicationLocksManager _applicationLocksManager;
        private readonly Random _rnd = new Random();
        private readonly object _sync = new object();

        private byte? _id;
        private Guid? _lockId;
        private bool _disposed;

        public AppLockIdentityServiceUniqueIdProvider(IApplicationLocksManager applicationLocksManager)
        {
            _applicationLocksManager = applicationLocksManager;
        }

        public byte GetUniqueId()
        {
            CheckNotDisposed();

            lock (_sync)
            {
                EnsureIdReserved();
            }

            // ReSharper disable once PossibleInvalidOperationException
            return _id.Value;
        }

        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_lockId != null)
            {
                _applicationLocksManager.ReleaseLock(_lockId.Value);
            }

            _disposed = true;
        }

        private void EnsureIdReserved()
        {
            if (_id != null && _lockId != null)
            {
                if (_applicationLocksManager.IsLockActive(_lockId.Value))
                {
                    return;
                }

                _applicationLocksManager.ReleaseLock(_lockId.Value);
            }

            var startId = _id ?? (byte)_rnd.Next(255);
            for (var i = 0; i < 255; i++)
            {
                var id = (byte)((startId + i) % 256);
                Guid lockId;
                if (_applicationLocksManager.AcquireLock(string.Format(IdAppLockTemplate, id), LockOwner.Session, LockScope.AllInstallations, TimeSpan.Zero, out lockId))
                {
                    _lockId = lockId;
                    _id = id;
                    return;
                }
            }

            throw new InvalidOperationException("Cannot acquire id application lock for identity service");
        }

        private void CheckNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}