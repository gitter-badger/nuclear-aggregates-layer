using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface IApplicationLocksReleaser
    {
        void Release(Guid scopeId, bool isScopeCompleted);
    }
}