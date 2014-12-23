using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksService : IInvariantSafeCrosscuttingService
    {
        ILockingScope Acquire(string lockName);
        ILockingScope Acquire(string lockName, TimeSpan timeout);
        bool TryAcquire(string lockName, out ILockingScope lockingScope);
        bool TryAcquire(string lockName, TimeSpan timeout, out ILockingScope lockingScope);
    }
}