using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLCore.OrderValidation.DI;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.AppFabric.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.API.WCF.OrderValidation.DI
{
    internal static class WcfOrderValidationRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                      .RequireZone<AggregatesZone>()
                                          .UseAnchor<BlCoreAggregatesAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                          .UseAnchor<BlCoreApiOrderValidationAssembly>()
                                          .UseAnchor<BlCoreOrderValidationAssembly>()
                                      .RequireZone<MetadataZone>()
                                          .UseAnchor<PlatformModelMetadataAssembly>()
                                      .RequireZone<AppFabricZone>()
                                          .UseAnchor<PlatformAppFabricAssembly>();
            }
        } 
    }
}