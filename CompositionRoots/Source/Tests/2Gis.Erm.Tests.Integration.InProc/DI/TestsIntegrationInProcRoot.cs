using DoubleGis.Erm.BL.Operations.DI;
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
using DoubleGis.Erm.BLCore.UI.Metadata.DI;
using DoubleGis.Erm.BLFlex.Aggregates.Global.DI;
using DoubleGis.Erm.BLFlex.Operations.Global.DI;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.BLQuerying.Operations.Listing.DI;
using DoubleGis.Erm.BLQuerying.UI.Metadata.DI;
using DoubleGis.Erm.Platform.Aggregates.DI;
using DoubleGis.Erm.Platform.API.Aggregates.DI;
using DoubleGis.Erm.Platform.Core;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.EntityFramework.DI;
using DoubleGis.Erm.Platform.Model.Metadata.DI;
using NuClear.Assembling.Zones;
using DoubleGis.Erm.Tests.Integration.InProc.DI.Zones;
using DoubleGis.Erm.Tests.Integration.InProc.DI.Zones.Parts;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal sealed class TestsIntegrationInProcRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<IntegrationTestsZone>()
                                            .UseAnchor<IntegrationTestsZonePartAssembly>()
                                      .RequireZone<AggregatesZone>()
                                            .UseAnchor<BlCoreAggregatesAssembly>()
                                            .UseAnchor<BlFlexAggregatesGlobalAssembly>()
                                            .UseAnchor<PlatformAggregatesAssembly>()
                                      .RequireZone<OperationsZone>()
                                            .UseAnchor<BlCoreApiOperationsAssembly>()
                                            .UseAnchor<BlCoreOperationsAssembly>()
                                            .UseAnchor<BlOperationsAssembly>()
                                            .UseAnchor<BlFlexOperationsGlobalAssembly>()
                                      .RequireZone<MoDiZone>()
                                            .UseAnchor<BlCoreApiModiAssembly>()
                                      .RequireZone<OperationsSpecialZone>()
                                            .UseAnchor<BlCoreApiOperationsSpecialAssembly>()
                                            .UseAnchor<BlCoreOperationsSpecialAssembly>()
                                            .UseAnchor<BlOperationsSpecialAssembly>()
                                      .RequireZone<PlatformZone>()
                                            .UseAnchor<BlCoreDalPersistenceServicesAssembly>()
                                            .UseAnchor<PlatformModelAssembly>()
                                            .UseAnchor<PlatformCoreAssembly>()
                                            .UseAnchor<PlatformModelEntityFrameworkAssembly>()
                                      .RequireZone<QueryingZone>()
                                            .UseAnchor<BlQueryingApiOperationsListingAssembly>()
                                            .UseAnchor<BlQueryingOperationsListingAssembly>()
                                      .RequireZone<ReleasingZone>()
                                            .UseAnchor<BlCoreApiReleasingAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                            .UseAnchor<BlCoreApiOrderValidationAssembly>()
                                            .UseAnchor<BlCoreOrderValidationAssembly>()
                                      .RequireZone<MetadataZone>()
                                            .UseAnchor<PlatformModelMetadataAssembly>()
                                            .UseAnchor<BlQueryingUIMetadataAssembly>()
                                            .UseAnchor<BLCore.UI.WPF.Client.DI.MetadataZonePartAssembly>()
                                            .UseAnchor<MetadataZonePartAssembly>()
                                            .UseAnchor<BlCoreUIMetadataAssembly>();
            }
        } 
    }
}