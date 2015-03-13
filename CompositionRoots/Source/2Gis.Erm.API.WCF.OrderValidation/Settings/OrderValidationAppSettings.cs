using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation.Settings;
using DoubleGis.Erm.BLCore.OrderValidation.Settings;
using DoubleGis.Erm.BLCore.OrderValidation.Settings.Xml;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using NuClear.IdentityService.Client.Settings;

using NuClear.Settings.API;

namespace DoubleGis.Erm.API.WCF.OrderValidation.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class OrderValidationAppSettings : SettingsContainerBase
    {
        public OrderValidationAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
               .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
               .Use(new OrderValidationSettingsAspect(AssociatedDeniedPositionsDescriptionsAccessor.GetPricePositionDescriptions()))
               .Use<OrderValidationCachingSettingsAspect>()
               .Use<CachingSettingsAspect>()
               .Use<OperationLoggingSettingsAspect>()
               .Use<IdentityServiceClientSettingsAspect>()
               .IfRequiredUseOperationLogging2ServiceBus();
        }
    }
}