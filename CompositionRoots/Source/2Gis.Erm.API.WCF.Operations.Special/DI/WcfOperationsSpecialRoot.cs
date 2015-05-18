using DoubleGis.Erm.BL.Aggregates.DI;
using DoubleGis.Erm.BL.Operations.DI;
using DoubleGis.Erm.BL.Operations.Special.DI;
using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLCore.Operations.Special;
using DoubleGis.Erm.BLCore.OrderValidation.DI;
using DoubleGis.Erm.Platform.Aggregates.DI;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.AppFabric.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.DI;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.EntityFramework.DI;
using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.API.WCF.Operations.Special.DI
{
    internal static class WcfOperationsSpecialRoot
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
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                          .UseAnchor<BlOperationsAssembly>()
                                      .RequireZone<OperationsSpecialZone>()
                                          .UseAnchor<BlCoreApiOperationsSpecialAssembly>()
                                          .UseAnchor<BlCoreOperationsSpecialAssembly>()
                                          .UseAnchor<BlOperationsSpecialAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                          .UseAnchor<BlCoreApiOrderValidationAssembly>()
                                          .UseAnchor<BlCoreOrderValidationAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<PlatformDalPersistenceServicesAssembly>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                          .UseAnchor<PlatformModelEntityFrameworkAssembly>()
                                      .RequireZone<AppFabricZone>()
                                          .UseAnchor<PlatformAppFabricAssembly>();
            }
        } 
    }
}