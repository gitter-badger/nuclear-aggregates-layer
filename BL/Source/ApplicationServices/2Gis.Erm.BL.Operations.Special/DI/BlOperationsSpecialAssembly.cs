using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BL.Operations.Special.DI
{
    public sealed class BlOperationsSpecialAssembly : IZoneAssembly<OperationsSpecialZone>,
                                                      IZoneAnchor<OperationsSpecialZone>,
                                                      IContainsType<IRequestHandler>
    {
    }
}