using System;

using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking;

namespace DoubleGis.Erm.Platform.Core.Locking
{
    public class ApplicationLocksService : IApplicationLocksService, IApplicationLocksManager
    {
        private readonly IApplicationLocksPersistenceService _applicationLocksPersistenceService;

        public ApplicationLocksService(IApplicationLocksPersistenceService applicationLocksPersistenceService)
        {
            _applicationLocksPersistenceService = applicationLocksPersistenceService;
        }

        public ILockingScope Acquire(string lockName)
        {
            return AcquireInternal(lockName, null);
        }

        public ILockingScope Acquire(string lockName, TimeSpan timeout)
        {
            return AcquireInternal(lockName, timeout);
        }

        public bool TryAcquire(string lockName, out ILockingScope lockingScope)
        {
            return TryAcquireInternal(lockName, null, out lockingScope);
        }

        public bool TryAcquire(string lockName, TimeSpan timeout, out ILockingScope lockingScope)
        {
            return TryAcquireInternal(lockName, timeout, out lockingScope);
        }

        public void Release(ITrackedLockingScope scope)
        {
            _applicationLocksPersistenceService.ReleaseLock(scope.Id);
        }

        private ILockingScope AcquireInternal(string lockName, TimeSpan? timeout)
        {
            ILockingScope lockingScope;
            if (!TryAcquireInternal(lockName, timeout, out lockingScope))
            {
                throw new InvalidOperationException(string.Format("Can't acquire lock named {0}", lockName));
            }

            return lockingScope;
        }

        private bool TryAcquireInternal(string lockName, TimeSpan? timeout, out ILockingScope lockingScope)
        {
            Guid lockId;
            if (_applicationLocksPersistenceService.AcquireLock(lockName, timeout ?? TimeSpan.Zero, out lockId))
            {
                lockingScope = new LockingScope(lockId, this);
                return true;
            }

            lockingScope = null;
            return false;
        }
    }
}