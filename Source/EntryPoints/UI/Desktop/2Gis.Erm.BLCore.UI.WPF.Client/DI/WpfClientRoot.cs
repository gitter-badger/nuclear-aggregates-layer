﻿using DoubleGis.Erm.BLCore.API.Operations.DI;
using DoubleGis.Erm.BLCore.API.Operations.Special.DI;
using DoubleGis.Erm.BLCore.API.OrderValidation.DI;
using DoubleGis.Erm.BLCore.API.Releasing.DI;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.DI;
using DoubleGis.Erm.Platform.Model.DI;
using DoubleGis.Erm.Platform.Model.Zones;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI
{
    internal static class WpfClientRoot
    {
        public static CompositionRoot Instance
        {
            get
            {
                return CompositionRoot.Config
                                      .RequireZone<OperationsZone>()
                                          .UseAnchor<BlCoreApiOperationsAssembly>()
                                      .RequireZone<OperationsSpecialZone>()
                                          .UseAnchor<BlCoreApiOperationsSpecialAssembly>()
                                      .RequireZone<PlatformZone>()
                                          .UseAnchor<PlatformModelAssembly>()
                                      .RequireZone<QueryingZone>()
                                          .UseAnchor<BlQueryingApiOperationsListingAssembly>()
                                      .RequireZone<ReleasingZone>()
                                          .UseAnchor<BlCoreApiReleasingAssembly>()
                                      .RequireZone<OrderValidationZone>()
                                          .UseAnchor<BlCoreApiOrderValidationAssembly>();
            }
        }
    }
}