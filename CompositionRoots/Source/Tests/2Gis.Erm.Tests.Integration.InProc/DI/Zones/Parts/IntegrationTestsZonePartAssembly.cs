using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.Zones.Parts
{
    public sealed class IntegrationTestsZonePartAssembly : IZoneAssembly<IntegrationTestsZone>,
                                              IZoneAnchor<IntegrationTestsZone>,
                                              IContainsType<IIntegrationTest>
    {
    }
}