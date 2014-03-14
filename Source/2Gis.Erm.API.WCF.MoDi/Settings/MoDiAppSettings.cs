using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.WCF.MoDi;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.Common.Settings;

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
                .Use<CachingSettingsAspect>();
        }
    }
}