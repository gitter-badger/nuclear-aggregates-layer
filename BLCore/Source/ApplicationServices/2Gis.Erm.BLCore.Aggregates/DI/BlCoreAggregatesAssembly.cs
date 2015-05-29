using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Aggregates;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.Aggregates.DI
{
    public sealed class BlCoreAggregatesAssembly : IZoneAssembly<AggregatesZone>,
                                                   IZoneAnchor<AggregatesZone>,
                                                   IContainsType<IAggregateReadModel>,
                                                   IContainsType<IAggregateService>,
                                                   IContainsType<ISimplifiedModelConsumer>,
                                                   IContainsType<ISimplifiedModelConsumerReadModel>
    {
    }
}