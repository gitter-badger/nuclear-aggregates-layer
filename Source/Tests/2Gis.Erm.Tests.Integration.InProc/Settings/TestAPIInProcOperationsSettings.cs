﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Tests.Integration.InProc.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class TestAPIInProcOperationsSettings : SettingsContainerBase
    {
        public TestAPIInProcOperationsSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<CachingSettingsAspect>()
                .Use<WarmClientProcessingSettingsAspect>()
                .Use<DebtProcessingSettingsAspect>()
                .Use(RequiredServices
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIIdentityServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>());
        }
    }
}