using NuClear.Model.Common.Entities.Aspects;
using NuClear.Assembling.Zones;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.DI
{
    public sealed class PlatformModelAssembly : IZoneAssembly<PlatformZone>,
                                                IZoneAnchor<PlatformZone>,
                                                IContainsType<IEntity>,
                                                IContainsType<IOperationIdentity>
    {
    }
}