using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using NuClear.Assembling.Zones;

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
