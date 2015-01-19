﻿using System;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksService : IInvariantSafeCrosscuttingService
    {
        ILockingScope Acquire(string lockName, LockOwner lockOwner, TimeSpan timeout);
        bool TryAcquire(string lockName, LockOwner lockOwner, out ILockingScope lockingScope);
    }
}