using DoubleGis.Erm.BLCore.API.MoDi.DI;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.MoDi.DI
{
    public sealed class BlCoreModiAssembly : IZoneAssembly<MoDiZone>,
                                             IZoneAnchor<MoDiZone>,
                                             IContainsType<IOperation>
    {
    }
}