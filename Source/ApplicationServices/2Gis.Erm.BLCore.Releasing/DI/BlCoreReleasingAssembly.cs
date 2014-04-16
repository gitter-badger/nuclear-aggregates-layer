using DoubleGis.Erm.BLCore.API.Releasing.DI;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.Releasing.DI
{
    public sealed class BlCoreReleasingAssembly : IZoneAssembly<ReleasingZone>,
                                                  IZoneAnchor<ReleasingZone>,
                                                  IContainsType<IOperation>,
                                                  IContainsType<IRequestHandler>
    {
    }
}