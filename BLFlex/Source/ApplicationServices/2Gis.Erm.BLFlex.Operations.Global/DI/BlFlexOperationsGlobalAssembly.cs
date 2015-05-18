using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DI
{
    public sealed class BlFlexOperationsGlobalAssembly : IZoneAssembly<OperationsZone>,
                                                         IZoneAnchor<OperationsZone>,
                                                         IContainsType<IAggregateReadModel>,
                                                         IContainsType<IOperation>,
                                                         IContainsType<IDeserializeServiceBusObjectService>,
                                                         IContainsType<IImportServiceBusDtoService>,
                                                         IContainsType<IRequestHandler>,
                                                         IContainsType<ISimplifiedModelConsumer>
    {
    }
}