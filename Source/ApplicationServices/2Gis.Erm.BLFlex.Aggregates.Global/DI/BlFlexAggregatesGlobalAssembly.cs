using DoubleGis.Erm.BLCore.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.DI
{
    public sealed class BlFlexAggregatesGlobalAssembly : IZoneAssembly<AggregatesZone>,
                                                         IZoneAnchor<AggregatesZone>,
                                                         IContainsType<IAggregateReadModel>,
                                                         IContainsType<IAggregateRepository>,
                                                         IContainsType<ISimplifiedModelConsumerReadModel>,
                                                         IContainsType<ISimplifiedModelConsumer>
    {
    }
}