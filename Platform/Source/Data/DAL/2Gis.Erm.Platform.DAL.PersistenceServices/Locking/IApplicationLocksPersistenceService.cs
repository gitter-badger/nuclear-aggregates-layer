using System;

using DoubleGis.Erm.Platform.API.Core.Locking;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking
{
    public interface IApplicationLocksPersistenceService
    {
        bool AcquireLock(string lockName, LockOwner lockOwner, LockScope lockScope, TimeSpan timeout, out Guid lockId);
        bool ReleaseLock(Guid lockId);
        bool IsLockActive(Guid lockId);
    }
}