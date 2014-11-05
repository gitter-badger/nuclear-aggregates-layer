﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;
using DoubleGis.Erm.Qds.Common.Settings;

namespace DoubleGis.Erm.WCF.BasicOperations.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class OperationsSettings : SettingsContainerBase, INotifiyProgressSettings
    {
        private readonly IntSetting _progressCallbackBatchSize = ConfigFileSetting.Int.Optional("ProgressCallbackBatchSize", 1);

        public OperationsSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            var connectionStrings = new ConnectionStringsSettingsAspect();

            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<DebtProcessingSettingsAspect>()
                .Use<NotificationsSettingsAspect>()
                .Use<CachingSettingsAspect>()
                .Use(new NestSettingsAspect(connectionStrings))
                .Use<ValidateFileSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus()
                .Use(RequiredServices
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIIdentityServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>()
                        .Is<APIWebClientServiceSettingsAspect>());
        }

        int INotifiyProgressSettings.ProgressCallbackBatchSize
        {
            get
            {
                return _progressCallbackBatchSize.Value;
            }
        }
    }
}