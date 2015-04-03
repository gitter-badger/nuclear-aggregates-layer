using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.BLCore.API.Releasing.DI
{
    public sealed class BlCoreApiReleasingAssembly : IZoneAssembly<ReleasingZone>,
                                                     IZoneAnchor<ReleasingZone>,
                                                     IContainsType<IOperation>
    {
    }
}