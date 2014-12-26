using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Checkin
{
    public class ServiceInstanceCheckinSettingsAspect : ISettingsAspect, IServiceInstanceCheckinSettings
    {
        private readonly Lazy<TimeSpan> _lazyCheckinInterval =
            new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(ConfigFileSetting.Int.Optional("CheckinIntervalMs", 15000).Value));

        private readonly Lazy<TimeSpan> _lazyCheckinTimeSafetyOffset =
            new Lazy<TimeSpan>(() => TimeSpan.FromMilliseconds(ConfigFileSetting.Int.Optional("CheckinTimeSafetyOffsetMs", 30000).Value));

        public TimeSpan CheckinInterval
        {
            get { return _lazyCheckinInterval.Value; }
        }

        public TimeSpan CheckinTimeSafetyOffset
        {
            get { return _lazyCheckinTimeSafetyOffset.Value; }
        }
    }
}