using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.Releasing.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLCore.Releasing.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.API.WCF.Releasing.DI
{
    internal static class WcfReleasingRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<AggregatesZone>()
                                          .UseAnchor<BlCoreAggregatesAssembly>()
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                      .RequireZone<ReleasingZone>()
                                          .UseAnchor<BlCoreApiReleasingAssembly>()
                                          .UseAnchor<BlCoreReleasingAssembly>()
                                      .RequireZone<MetadataZone>()
                                          .UseAnchor<PlatformModelMetadataAssembly>();
            }
        }
    }
}