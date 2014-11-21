using DevUtils.ETWIMBA.Diagnostics.Counters;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Counters
{
    public static partial class Counters
    {
        [CounterSet(CounterSetInstances.Multiple,
            Name = "Erm_OrderValidation_Rules",
            Description = "Erm OrderValidation Rules")]
        public enum Rules
        {
            [Counter(CounterType.PerfCounterRawcount,
                DefaultScale = 1,
                Name = "Total execution time",
                Description = "Rule total execution since time after validation service host computer started",
                DetailLevel = CounterDetailLevel.Standard)]
            TotalTimeSec = 1,

            [Counter(CounterType.PerfCounterRawcount,
               DefaultScale = 1,
               Name = "Total orders count",
               Description = "Rule total passed orders count since time after validation service host computer started",
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
                Name = "Validation results cache utilization. Percentage",
                Description = "Validation results cache utilization",
                DetailLevel = CounterDetailLevel.Standard)]
            ValidationResultsCacheUtilizationPercentage = 4,

            [Counter(CounterType.PerfCounterRawcount,
                DefaultScale = 1,
                Name = "Execution time relative to entire validation session time. Percentage",
                Description = "Execution time relative to entire validation session time",
                DetailLevel = CounterDetailLevel.Standard)]
            AvgConsumedTimePercentage = 5,
        }
    }
}
