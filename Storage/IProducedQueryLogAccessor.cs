using System;

namespace NuClear.Storage
{
    public interface IProducedQueryLogAccessor
    {
        Action<string> Log { get; }
    }
}