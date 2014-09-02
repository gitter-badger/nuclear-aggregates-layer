using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IProducedQueryLogAccessor
    {
        Action<string> Log { get; }
    }
}