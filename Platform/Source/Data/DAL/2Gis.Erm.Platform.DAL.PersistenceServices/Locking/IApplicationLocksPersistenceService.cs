using System;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.Locking
{
    public interface IApplicationLocksPersistenceService
    {
        bool AcquireLock(string lockName, TimeSpan timeout, out Guid lockId);
        bool ReleaseLock(Guid lockId);
    }
}