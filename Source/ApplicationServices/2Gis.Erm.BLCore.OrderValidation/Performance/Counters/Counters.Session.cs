using DevUtils.ETWIMBA.Diagnostics.Counters;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Counters
{
    public static partial class Counters
    {
        [CounterSet(CounterSetInstances.Single,
            Name = "Erm_OrderValidation_Sessions",
            Description = "Erm OrderValidation Sessions")]
        public enum Sessions
        {
            [Counter(CounterType.PerfCounterRawcount,
                DefaultScale = 1,
                Name = "Total execution time",
                Description = "Sessions total execution since time after validation service host computer started",
                DetailLevel = CounterDetailLevel.Standard)]
            TotalTimeSec = 1,

            [Counter(CounterType.PerfCounterRawcount,
               DefaultScale = 1,
               Name = "Total orders count",
               Description = "Sessions total passed orders count since time after validation service host computer started",
               DetailLevel = CounterDetailLevel.Standard)]
            TotalOrdersCount = 2,

            [Counter(CounterType.PerfCounterRawcount,
               DefaultScale = 1,
               Name = "Avg orders validation rate. Orders/sec",
               Description = "Average orders validation rate since time after validation service host computer started",
               DetailLevel = CounterDetailLevel.Standard)]
            AvgValidationRateOrdersPerSec = 3,

            [Counter(CounterType.PerfCounterRawcount,
               DefaultScale = 1,
               Name = "Executed validation sessions count",
               Description = "Executed validation sessions count",
               DetailLevel = CounterDetailLevel.Standard)]
            TotalCount = 4,

            [Counter(CounterType.PerfCounterRawcount,
               DefaultScale = 1,
               Name = "Executing validation sessions count",
               Description = "Executing validation sessions count",
               DetailLevel = CounterDetailLevel.Standard)]
            ActiveCount = 5,
        }
    }
}
