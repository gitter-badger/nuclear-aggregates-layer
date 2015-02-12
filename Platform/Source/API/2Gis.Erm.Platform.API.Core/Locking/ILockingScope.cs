using System;

namespace DoubleGis.Erm.Platform.API.Core.Locking
{
    public interface ILockingScope : IDisposable
    {
        void Complete();
    }
}