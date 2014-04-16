using DoubleGis.Erm.BL.Operations.Special.DI;
using DoubleGis.Erm.BLCore.Aggregates.DI;
using DoubleGis.Erm.BLCore.API.MoDi.DI;
using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.API.Releasing.DI;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.DI;
using DoubleGis.Erm.BLCore.Operations.DI;
using DoubleGis.Erm.BLCore.Operations.Special;
using DoubleGis.Erm.BLCore.OrderValidation.DI;
using DoubleGis.Erm.BLFlex.Aggregates.Global.DI;
using DoubleGis.Erm.BLFlex.Operations.Global.DI;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.BLQuerying.Operations.Listing.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Zones;
using DoubleGis.Erm.Tests.Integration.InProc.DI;

namespace DoubleGis.Erm.Tests.Integration.InProc
{
    internal sealed class TestsIntegrationInProcRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<IntegrationTestsZone>()
                                          .UseAnchor<TestIntegrationInProcAssembly>()
                                      //.RequireZone<WebMvcZone>()
                                      //    .UseAnchor<BlCoreUiWebMvcAssembly>()
                                      //    .UseAnchor<BlUiWebMvcAssembly>()
                                      //    .UseAnchor<BlFlexUiWebMvcAssembly>()
                                      //    .UseAnchor<PlatformUiWebMvcAssembly>()
                                      .RequireZone<AggregatesZone>()
                                          .UseAnchor<BlCoreAggregatesAssembly>()
                                          .UseAnchor<BlFlexAggregatesGlobalAssembly>()
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                          .UseAnchor<BlCoreOperationsAssembly>()
                                          .UseAnchor<BlFlexOperationsGlobalAssembly>()

                                      .RequireZone<MoDiZone>()
                                          .UseAnchor<BlCoreApiModiAssembly>()
                                          //.UseAnchor<BlCoreModiAssembly>()
                                      .RequireZone<OperationsSpecialZone>()
                                          .UseAnchor<BlCoreApiOperationsSpecialAssembly>()
                                          .UseAnchor<BlCoreOperationsSpecialAssembly>()
                                          .UseAnchor<BlOperationsSpecialAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                          //.UseAnchor<PlatformDalPersistenceServicesAssembly>()
                                          .UseAnchor<PlatformModelAssembly>()
                                          .UseAnchor<PlatformCoreAssembly>()
                                      .RequireZone<QueryingZone>()
                                          .UseAnchor<BlQueryingApiOperationsListingAssembly>()
                                          .UseAnchor<BlQueryingOperationsListingAssembly>()
                                          //.UseAnchor<QdsOperationsAssembly>()
                                      .RequireZone<ReleasingZone>()
                                          .UseAnchor<BlCoreApiReleasingAssembly>()
                                          //.UseAnchor<BlCoreReleasingAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                          .UseAnchor<BlCoreApiOrderValidationAssembly>()
                                          .UseAnchor<BlCoreOrderValidationAssembly>();
            }
        } 
    }
}