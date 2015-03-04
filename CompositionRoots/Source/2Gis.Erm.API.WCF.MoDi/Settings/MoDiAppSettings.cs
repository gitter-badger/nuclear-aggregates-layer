using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.WCF.MoDi;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.API.WCF.MoDi.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class MoDiAppSettings : SettingsContainerBase
    {
        public MoDiAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<MoneyDistributionSettingsAspect>()
                .Use<CachingSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus();
        }
    }
}