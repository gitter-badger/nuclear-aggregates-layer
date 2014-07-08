using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.Aggregates.DI
{
    public sealed class BlCoreAggregatesAssembly : IZoneAssembly<AggregatesZone>,
                                                   IZoneAnchor<AggregatesZone>,
                                                   IContainsType<IAggregateReadModel>,
                                                   IContainsType<IAggregateRepository>,
                                                   IContainsType<ISimplifiedModelConsumer>,
                                                   IContainsType<ISimplifiedModelConsumerReadModel>
    {
    }
}