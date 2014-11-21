using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public interface IEnsureOrderExportedStrategyContainer
    {
        IEnumerable<IEnsureOrderExportedStrategy> Strategies { get; }
    }
}