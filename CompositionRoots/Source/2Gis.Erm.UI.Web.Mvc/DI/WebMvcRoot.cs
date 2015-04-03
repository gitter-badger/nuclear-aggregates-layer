using DoubleGis.Erm.BL.Aggregates.DI;
using DoubleGis.Erm.BL.Operations.DI;
using DoubleGis.Erm.BL.Operations.Special.DI;
using DoubleGis.Erm.BL.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.API.Releasing.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLCore.Operations.Special;
using DoubleGis.Erm.BLCore.OrderValidation.DI;
using DoubleGis.Erm.BLCore.Releasing.DI;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.DI;
using DoubleGis.Erm.BLFlex.Aggregates.Global.DI;
using DoubleGis.Erm.BLFlex.Operations.Global.DI;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.DI;
using DoubleGis.Erm.Platform.Aggregates.DI;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.AppFabric.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.DI;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.EntityFramework.DI;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;

using NuClear.Assembling.Zones;

namespace DoubleGis.Erm.UI.Web.Mvc.DI
{
    internal static class WebMvcRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<WebMvcZone>()
                                          .UseAnchor<BlCoreUiWebMvcAssembly>()
                                          .UseAnchor<BlUiWebMvcAssembly>()
                                          .UseAnchor<BlFlexUiWebMvcAssembly>()
                                          .UseAnchor<PlatformUiWebMvcAssembly>()
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
                                      .RequireZone<OperationsSpecialZone>()
                                          .UseAnchor<BlCoreApiOperationsSpecialAssembly>()
                                          .UseAnchor<BlCoreOperationsSpecialAssembly>()
                                          .UseAnchor<BlOperationsSpecialAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                          .UseAnchor<BlCoreApiOrderValidationAssembly>()
                                          .UseAnchor<BlCoreOrderValidationAssembly>()
                                      .RequireZone<ReleasingZone>()
                                          .UseAnchor<BlCoreApiReleasingAssembly>()
                                          .UseAnchor<BlCoreReleasingAssembly>()
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