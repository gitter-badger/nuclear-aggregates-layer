using System;

using DoubleGis.Erm.TaskService.Settings;

namespace DoubleGis.Erm.TaskService.Tests.Unit.Fakes
{
    public sealed class FakeBatchIndexingSettings : IBatchIndexingSettings
    {
        public FakeBatchIndexingSettings(int sleepTimeMilliseconds, int stopTimeoutMilliseconds)
            : this(TimeSpan.FromMilliseconds(sleepTimeMilliseconds), TimeSpan.FromMilliseconds(stopTimeoutMilliseconds))
        {
        }

        public FakeBatchIndexingSettings(TimeSpan sleepTime, TimeSpan stopTimeout)
        {
            SleepTime = sleepTime;
            StopTimeout = stopTimeout;
        }

        public TimeSpan SleepTime { get; set; }
        public TimeSpan StopTimeout { get; set; }
    }
}