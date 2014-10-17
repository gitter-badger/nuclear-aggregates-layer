using System;

using DevUtils.ETWIMBA.Diagnostics.Counters;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions
{
    public interface IOrderValidationDiagnosticSession : IDisposable
    {
        CounterSetInstance<Counters.Counters.Sessions> Get();
        CounterSetInstance<Counters.Counters.RuleGroups> Get(OrderValidationRuleGroup ruleGroup);
        CounterSetInstance<Counters.Counters.Rules> Get(Type ruleType);
    }
}