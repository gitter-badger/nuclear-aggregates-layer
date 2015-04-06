using System;

using DoubleGis.Erm.Platform.API.Core.Locking;


using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Locking
{
    public class ApplicationLocksService : IApplicationLocksService, IApplicationLocksReleaser
    {
        private readonly IApplicationLocksManager _applicationLocksManager;
        private readonly ITracer _tracer;

        public ApplicationLocksService(IApplicationLocksManager applicationLocksManager, ITracer tracer)
        {
            _applicationLocksManager = applicationLocksManager;
            _tracer = tracer;
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
                _tracer.Warn("Locking scope has not been explicitly completed");
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