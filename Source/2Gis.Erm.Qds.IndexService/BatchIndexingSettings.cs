using System;

namespace DoubleGis.Erm.Qds.IndexService
{
    public class BatchIndexingSettings
    {
        public BatchIndexingSettings(int sleepTimeMilliseconds, int stopTimeoutMilliseconds)
            : this(TimeSpan.FromMilliseconds(sleepTimeMilliseconds), TimeSpan.FromMilliseconds(stopTimeoutMilliseconds))
        {
        }

        public BatchIndexingSettings(TimeSpan sleepTime, TimeSpan stopTimeout)
        {
            SleepTime = sleepTime;
            StopTimeout = stopTimeout;
        }

        public TimeSpan SleepTime { get; set; }
        public TimeSpan StopTimeout { get; set; }
    }
}