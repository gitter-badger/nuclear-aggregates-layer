using DoubleGis.Erm.BL.Aggregates.DI;
using DoubleGis.Erm.BL.Operations.DI;
using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLFlex.Aggregates.Global.DI;
using DoubleGis.Erm.BLFlex.Operations.Global.DI;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.BLQuerying.Operations.Listing.DI;
using DoubleGis.Erm.Platform.Aggregates.DI;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.AppFabric.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.DI;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.EntityFramework.DI;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Qds.Operations.DI;

namespace DoubleGis.Erm.WCF.BasicOperations.DI
{
    internal static class WcfOperationsRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<AggregatesZone>()
                                          .UseAnchor<PlatformAggregatesAssembly>()
                                          .UseAnchor<BlCoreAggregatesAssembly>()
                                          .UseAnchor<BlAggregatesAssembly>()
                                          .UseAnchor<BlFlexAggregatesGlobalAssembly>()
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                          .UseAnchor<BlOperationsAssembly>()
                                          .UseAnchor<BlFlexOperationsGlobalAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<PlatformDalPersistenceServicesAssembly>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                          .UseAnchor<PlatformModelEntityFrameworkAssembly>()
                                      .RequireZone<QueryingZone>()
                                          .UseAnchor<BlQueryingApiOperationsListingAssembly>()
                                          .UseAnchor<BlQueryingOperationsListingAssembly>()
                                          .UseAnchor<QdsOperationsAssembly>()
                                      .RequireZone<MetadataZone>()
                                          .UseAnchor<PlatformModelMetadataAssembly>()
                                      .RequireZone<AppFabricZone>()
                                          .UseAnchor<PlatformAppFabricAssembly>();
            }
        } 
    }
}