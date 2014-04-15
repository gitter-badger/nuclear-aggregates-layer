using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.MoDi.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.MoDi.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.API.WCF.MoDi.DI
{
    internal static class WcfMoDiRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<AggregatesZone>()
                                          .UseAnchor<BlCoreAggregatesAssembly>()
                                      .RequireZone<MoDiZone>()
                                          .UseAnchor<BlCoreApiModiAssembly>()
                                          .UseAnchor<BlCoreModiAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>();
            }
        }
    }
}