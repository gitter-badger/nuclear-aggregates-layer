using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal sealed class TestIntegrationInProcAssembly : IZoneAssembly<IntegrationTestsZone>,
                                                          IZoneAnchor<IntegrationTestsZone>,
                                                          IContainsType<IIntegrationTest>
    {
    }
}