using NuClear.Assembling.Zones;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.Zones.Parts
{
    public sealed class IntegrationTestsZonePartAssembly : IZoneAssembly<IntegrationTestsZone>,
                                              IZoneAnchor<IntegrationTestsZone>,
                                              IContainsType<IIntegrationTest>
    {
    }
}