using System.Diagnostics;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback
{
    internal sealed class RuleIndicators
    {
        public double ExecutionTimeSec { get; set; }
        public int ValidatedOrdersCount { get; set; }
    }
}