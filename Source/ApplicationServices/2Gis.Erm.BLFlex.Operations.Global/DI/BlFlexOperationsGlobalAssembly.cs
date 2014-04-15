using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DI
{
    public sealed class BlFlexOperationsGlobalAssembly : IZoneAssembly<OperationsZone>,
                                                         IZoneAnchor<OperationsZone>,
                                                         IContainsType<IAggregateReadModel>,
                                                         IContainsType<IOperation>,
                                                         IContainsType<IRequestHandler>,
                                                         IContainsType<ISimplifiedModelConsumer>
    {
    }
}