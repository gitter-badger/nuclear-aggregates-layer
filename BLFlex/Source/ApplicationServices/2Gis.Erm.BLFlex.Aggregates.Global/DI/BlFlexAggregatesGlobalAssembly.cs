using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Aggregates;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.DI
{
    public sealed class BlFlexAggregatesGlobalAssembly : IZoneAssembly<AggregatesZone>,
                                                         IZoneAnchor<AggregatesZone>,
                                                         IContainsType<IAggregateReadModel>,
                                                         IContainsType<IAggregateService>,
                                                         IContainsType<ISimplifiedModelConsumerReadModel>,
                                                         IContainsType<IDeserializeServiceBusObjectService>,
                                                         IContainsType<IImportServiceBusDtoService>,
                                                         IContainsType<ISimplifiedModelConsumer>
    {
    }
}