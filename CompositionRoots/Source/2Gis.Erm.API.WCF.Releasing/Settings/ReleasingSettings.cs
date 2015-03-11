﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.NuClear.IdentityService.Client.Settings;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.API.WCF.Releasing.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class ReleasingSettings : SettingsContainerBase
    {
        public ReleasingSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
               .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
               .Use<IntegrationSettingsAspect>()
               .Use<CachingSettingsAspect>()
               .Use<OperationLoggingSettingsAspect>()
               .Use<IdentityServiceClientSettingsAspect>()
               .IfRequiredUseOperationLogging2ServiceBus()
               .Use(RequiredServices.Is<APIOrderValidationServiceSettingsAspect>());
        }
    }
}