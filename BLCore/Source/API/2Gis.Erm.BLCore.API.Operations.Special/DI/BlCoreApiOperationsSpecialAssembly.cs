using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.DI
{
    public sealed class BlCoreApiOperationsSpecialAssembly : IZoneAssembly<OperationsSpecialZone>,
                                                             IZoneAnchor<OperationsSpecialZone>,
                                                             IContainsType<IOperation>
    {
    }
}