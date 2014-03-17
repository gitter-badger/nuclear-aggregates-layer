using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Qds.API.Core.Settings;

namespace DoubleGis.Erm.Qds.IndexService.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class IndexServiceAppSettings : SettingsContainerBase, IBatchIndexingSettings
    {
        private readonly IntSetting _batchIndexingSleep = ConfigFileSetting.Int.Optional("BatchIndexingSleep", 50);
        private readonly IntSetting _batchIndexingStopTimeout = ConfigFileSetting.Int.Optional("BatchIndexingStopTimeout", 1000);

        public IndexServiceAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            var connectionStrings = new ConnectionStringsSettingsAspect();
            Aspects
               .Use(connectionStrings)
               .Use(new GlobalizationAspect(supportedBusinessModelIndicators))
               .Use<EnvironmentsAspect>()
               .Use<CachingSettingsAspect>()
               .Use(new SearchSettingsAspect(connectionStrings));
        }

        TimeSpan IBatchIndexingSettings.SleepTime 
        {
            get { return TimeSpan.FromMilliseconds(_batchIndexingSleep.Value); }
        }

        TimeSpan IBatchIndexingSettings.StopTimeout
        {
            get { return TimeSpan.FromMilliseconds(_batchIndexingStopTimeout.Value); }
        }
    }
}