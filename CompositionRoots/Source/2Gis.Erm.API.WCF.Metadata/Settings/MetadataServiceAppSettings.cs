using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Identities;

using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.API.WCF.Metadata.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class MetadataServiceAppSettings : SettingsContainerBase, IIdentityProviderSettings
    {
        private readonly IntSetting _identityServiceUniqueId = ConfigFileSetting.Int.Required("IdentityServiceUniqueId");

        public MetadataServiceAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<CachingSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus()
                .Use(RequiredServices
                        .Is<APIIntrospectionServiceSettingsAspect>()
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIIdentityServiceSettingsAspect>()
                        .Is<APIOperationsServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>()
                        .Is<APIReleasingServiceSettingsAspect>()
                        .Is<APISpecialOperationsServiceSettingsAspect>());
        }

        int IIdentityProviderSettings.IdentityServiceUniqueId
        {
            get
            {
                return _identityServiceUniqueId.Value;
            }
        }
    }
}