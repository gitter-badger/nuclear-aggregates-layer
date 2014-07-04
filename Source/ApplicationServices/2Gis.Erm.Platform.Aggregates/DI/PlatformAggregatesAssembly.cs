using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.Aggregates.DI
{
    public sealed class PlatformAggregatesAssembly : IZoneAssembly<AggregatesZone>,
                                                   IZoneAnchor<AggregatesZone>,
                                                   IContainsType<ISimplifiedModelConsumer>,
                                                   IContainsType<ISimplifiedModelConsumerReadModel>
    {
    }
}
