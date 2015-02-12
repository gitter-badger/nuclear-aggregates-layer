using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksManager
    {
        bool AcquireLock(string lockName, LockOwner lockOwner, LockScope lockScope, TimeSpan timeout, out Guid lockId);
        bool ReleaseLock(Guid lockId);
        bool IsLockActive(Guid lockId);
    }
}