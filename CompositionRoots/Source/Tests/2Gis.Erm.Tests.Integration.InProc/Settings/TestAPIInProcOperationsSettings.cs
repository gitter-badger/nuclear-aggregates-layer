using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Metadata.Settings;

using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class TestAPIInProcOperationsSettings : SettingsContainerBase, IOrderProcessingSettings, IIntegrationLocalizationSettings
    {
        private readonly StringSetting _basicLanguage = ConfigFileSetting.String.Required("BasicLanguage");
        private readonly StringSetting _reserveLanguage = ConfigFileSetting.String.Required("ReserveLanguage");
        private readonly StringSetting _regionalTerritoryLocaleSpecificWord = ConfigFileSetting.String.Required("RegionalTerritoryLocaleSpecificWord");

        private readonly IntSetting _orderRequestProcessingHoursAmount = ConfigFileSetting.Int.Required("OrderRequestProcessingHoursAmount");

        public TestAPIInProcOperationsSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<CachingSettingsAspect>()
                .Use<WarmClientProcessingSettingsAspect>()
                .Use<DebtProcessingSettingsAspect>()
                .Use<IntegrationSettingsAspect>()
                .Use<NotificationsSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus()
                .Use<PerformedOperationsTransportSettingsAspect>()
                .IfRequiredUsePerformedOperationsFromServiceBusAspect()
                .Use(RequiredServices
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIIdentityServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>());
        }

        public string BasicLanguage
        {
            get { return _basicLanguage.Value; }
        }

        public string ReserveLanguage
        {
            get { return _reserveLanguage.Value; }
        }

        public string RegionalTerritoryLocaleSpecificWord
        {
            get { return _regionalTerritoryLocaleSpecificWord.Value; }
        }

        int IOrderProcessingSettings.OrderRequestProcessingHoursAmount
        {
            get { return _orderRequestProcessingHoursAmount.Value; }
        }
    }
}