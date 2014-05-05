using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.MoDi.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.MoDi.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLFlex.Aggregates.Global.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
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
                                          .UseAnchor<BlFlexAggregatesGlobalAssembly>()
                                      .RequireZone<MoDiZone>()
                                          .UseAnchor<BlCoreApiModiAssembly>()
                                          .UseAnchor<BlCoreModiAssembly>()
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                      .RequireZone<MetadataZone>()
                                          .UseAnchor<PlatformModelMetadataAssembly>();
            }
        }
    }
}