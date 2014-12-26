using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Checkin
{
    public interface IServiceInstanceCheckinSettings : ISettings
    {
        TimeSpan CheckinInterval { get; }
        TimeSpan CheckinTimeSafetyOffset { get; }
    }
}