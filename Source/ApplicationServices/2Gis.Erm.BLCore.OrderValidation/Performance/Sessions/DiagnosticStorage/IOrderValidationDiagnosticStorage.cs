using System;

using DevUtils.ETWIMBA.Diagnostics.Counters;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.DiagnosticStorage
{
    public interface IOrderValidationDiagnosticStorage : IDisposable
    {
        CounterSetInstance<Counters.Counters.Sessions> Session { get; }
        CounterSetInstance<Counters.Counters.RuleGroups> this[OrderValidationRuleGroup ruleGroup] { get; }
        CounterSetInstance<Counters.Counters.Rules> this[Type ruleType] { get; }
    }
}