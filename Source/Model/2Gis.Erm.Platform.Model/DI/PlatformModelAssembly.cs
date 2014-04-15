using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.Platform.Model.DI
{
    public sealed class PlatformModelAssembly : IZoneAssembly<PlatformZone>,
                                                IZoneAnchor<PlatformZone>,
                                                IContainsType<IEntity>,
                                                IContainsType<IOperationIdentity>
    {
    }
}