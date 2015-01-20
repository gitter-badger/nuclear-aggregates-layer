using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksService : IInvariantSafeCrosscuttingService
    {
        ILockingScope Acquire(string lockName, LockOwner lockOwner, LockScope lockScope, TimeSpan timeout);
        bool TryAcquire(string lockName, LockOwner lockOwner, LockScope lockScope, out ILockingScope lockingScope);
    }
}