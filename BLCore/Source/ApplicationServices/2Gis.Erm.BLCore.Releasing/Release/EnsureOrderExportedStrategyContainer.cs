using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class EnsureOrderExportedStrategyContainer : IEnsureOrderExportedStrategyContainer
    {
        public EnsureOrderExportedStrategyContainer(IEnumerable<IEnsureOrderExportedStrategy> strategies)
        {
            Strategies = strategies;
        }

        public IEnumerable<IEnsureOrderExportedStrategy> Strategies { get; private set; }
    }
}