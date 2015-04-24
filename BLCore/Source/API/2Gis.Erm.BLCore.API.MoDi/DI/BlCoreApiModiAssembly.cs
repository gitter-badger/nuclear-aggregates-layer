using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.API.MoDi.DI
{
    public sealed class BlCoreApiModiAssembly : IZoneAssembly<MoDiZone>,
                                                IZoneAnchor<MoDiZone>,
                                                IContainsType<IOperation>

    {
    }
}