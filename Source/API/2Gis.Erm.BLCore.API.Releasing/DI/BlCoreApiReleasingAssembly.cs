using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.API.Releasing.DI
{
    public sealed class BlCoreApiReleasingAssembly : IZoneAssembly<ReleasingZone>,
                                                     IZoneAnchor<ReleasingZone>,
                                                     IContainsType<IOperation>
    {
    }
}