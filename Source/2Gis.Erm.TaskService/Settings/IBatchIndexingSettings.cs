using System;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.TaskService.Settings
{
    public interface IBatchIndexingSettings : ISettings
    {
        TimeSpan SleepTime { get; }
        TimeSpan StopTimeout { get; }
    }
}