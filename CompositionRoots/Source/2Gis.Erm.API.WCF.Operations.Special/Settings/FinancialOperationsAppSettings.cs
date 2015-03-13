using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using NuClear.IdentityService.Client.Settings;

using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.API.WCF.Operations.Special.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class FinancialOperationsAppSettings : SettingsContainerBase, IOrderProcessingSettings
    {
        private readonly IntSetting _orderRequestProcessingHoursAmount = ConfigFileSetting.Int.Required("OrderRequestProcessingHoursAmount");

        public FinancialOperationsAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<DebtProcessingSettingsAspect>()
                .Use<NotificationsSettingsAspect>()
                .Use<CachingSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .Use<IdentityServiceClientSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus()
                .Use(RequiredServices
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>()
                        .Is<APIWebClientServiceSettingsAspect>());
        }

        int IOrderProcessingSettings.OrderRequestProcessingHoursAmount
        {
            get { return _orderRequestProcessingHoursAmount.Value; }
        }
    }
}