using DoubleGis.Erm.BLCore.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BL.Aggregates.DI
{
    public sealed class BlAggregatesAssembly : IZoneAssembly<AggregatesZone>,
                                               IZoneAnchor<AggregatesZone>,
                                               IContainsType<IAggregateReadModel>,
                                               IContainsType<IAggregateRepository>,
                                               IContainsType<ISimplifiedModelConsumer>,
                                               IContainsType<ISimplifiedModelConsumerReadModel>
    {
    }
}
