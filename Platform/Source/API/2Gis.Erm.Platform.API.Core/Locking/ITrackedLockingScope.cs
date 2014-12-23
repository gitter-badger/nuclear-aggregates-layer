using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface ITrackedLockingScope
    {
        Guid Id { get; }
    }
}