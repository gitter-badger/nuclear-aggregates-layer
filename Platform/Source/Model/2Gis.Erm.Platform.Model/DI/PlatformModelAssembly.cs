using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Zones;

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