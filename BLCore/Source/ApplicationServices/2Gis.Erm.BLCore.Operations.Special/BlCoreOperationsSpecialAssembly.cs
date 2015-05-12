using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Aggregates;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.Operations.Special
{
    public sealed class BlCoreOperationsSpecialAssembly : IZoneAssembly<OperationsSpecialZone>,
                                                          IZoneAnchor<OperationsSpecialZone>,
                                                          IContainsType<IAggregateReadModel>,
                                                          IContainsType<IOperation>,
                                                          IContainsType<IRequestHandler>,
                                                          IContainsType<ISimplifiedModelConsumer>
    {
    }
}