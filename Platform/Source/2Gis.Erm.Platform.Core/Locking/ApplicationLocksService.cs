using System;

using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.Core.Locking
{
    public class ApplicationLocksService : IApplicationLocksService, IApplicationLocksReleaser
    {
        private readonly IApplicationLocksManager _applicationLocksManager;
        private readonly ICommonLog _logger;

        public ApplicationLocksService(IApplicationLocksManager applicationLocksManager, ICommonLog logger)
        {
            _applicationLocksManager = applicationLocksManager;
            _logger = logger;
        }

        public ILockingScope Acquire(string lockName, LockOwner lockOwner, LockScope lockScope, TimeSpan timeout)
        {
            return AcquireInternal(lockName, lockOwner, timeout, lockScope);
        }

        public bool TryAcquire(string lockName, LockOwner lockOwner, LockScope lockScope, out ILockingScope lockingScope)
        {
            return TryAcquireInternal(lockName, lockOwner, null, out lockingScope, lockScope);
        }

        public void Release(Guid scopeId, bool isScopeCompleted)
        {
            if (!isScopeCompleted)
            {
                _logger.WarnEx("Locking scope has not been explicitly completed");
            }

            _applicationLocksManager.ReleaseLock(scopeId);
        }

        private ILockingScope AcquireInternal(string lockName, LockOwner lockOwner, TimeSpan? timeout, LockScope lockScope)
        {
            ILockingScope lockingScope;
            if (!TryAcquireInternal(lockName, lockOwner, timeout, out lockingScope, lockScope))
            {
                throw new InvalidOperationException(string.Format("Can't acquire lock named {0}", lockName));
            }

            return lockingScope;
        }

        private bool TryAcquireInternal(string lockName, LockOwner lockOwner, TimeSpan? timeout, out ILockingScope lockingScope, LockScope lockScope)
        {
            Guid lockId;
            if (_applicationLocksManager.AcquireLock(lockName, lockOwner, lockScope, timeout ?? TimeSpan.Zero, out lockId))
            {
                lockingScope = new LockingScope(lockId, this);
                return true;
            }

            lockingScope = null;
            return false;
        }
    }
}