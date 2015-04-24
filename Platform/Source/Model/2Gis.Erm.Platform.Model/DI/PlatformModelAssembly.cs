using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Platform.Model.DI
{
    public sealed class PlatformModelAssembly : IZoneAssembly<PlatformZone>,
                                                IZoneAnchor<PlatformZone>,
                                                IContainsType<IEntity>,
                                                IContainsType<IOperationIdentity>
    {
    }
}