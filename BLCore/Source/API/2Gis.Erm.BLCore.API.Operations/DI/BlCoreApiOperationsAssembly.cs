using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.API.Operations.DI
{
    public sealed class BlCoreApiOperationsAssembly : IZoneAssembly<OperationsZone>,
                                                      IZoneAnchor<OperationsZone>,
                                                      IContainsType<IOperation>,
                                                      IContainsType<IServiceBusFlow>,
                                                      IContainsType<IServiceBusDto>
    {
    }
}