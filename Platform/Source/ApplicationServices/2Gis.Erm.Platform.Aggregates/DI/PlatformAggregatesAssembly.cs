using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Platform.Aggregates.DI
{
    public sealed class PlatformAggregatesAssembly : IZoneAssembly<AggregatesZone>,
                                                   IZoneAnchor<AggregatesZone>,
                                                   IContainsType<ISimplifiedModelConsumer>,
                                                   IContainsType<ISimplifiedModelConsumerReadModel>
    {
    }
}
