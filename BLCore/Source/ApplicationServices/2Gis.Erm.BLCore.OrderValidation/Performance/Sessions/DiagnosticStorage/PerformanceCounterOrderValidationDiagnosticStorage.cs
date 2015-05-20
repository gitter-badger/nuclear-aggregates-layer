using System;
using System.Collections.Generic;
using System.Linq;

using DevUtils.ETWIMBA.Diagnostics.Counters;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;

using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.DiagnosticStorage
{
    public sealed partial class PerformanceCounterOrderValidationDiagnosticStorage : IOrderValidationDiagnosticStorage
    {
        private readonly CounterSet<Counters.Counters.Sessions> _sessionsCounterSet = new CounterSet<Counters.Counters.Sessions>();
        private readonly CounterSet<Counters.Counters.RuleGroups> _ruleGroupsCounterSet = new CounterSet<Counters.Counters.RuleGroups>();
        private readonly CounterSet<Counters.Counters.Rules> _rulesCounterSet = new CounterSet<Counters.Counters.Rules>();

        private readonly CounterSetInstance<Counters.Counters.Sessions> _sessionsCounterSetInstance;
        private readonly Dictionary<OrderValidationRuleGroup, CounterSetInstance<Counters.Counters.RuleGroups>> _ruleGroupsCounterSetInstances = 
            new Dictionary<OrderValidationRuleGroup, CounterSetInstance<Counters.Counters.RuleGroups>>();
        private readonly Dictionary<Type, CounterSetInstance<Counters.Counters.Rules>> _rulesCounterSetInstances = 
            new Dictionary<Type, CounterSetInstance<Counters.Counters.Rules>>();

        public PerformanceCounterOrderValidationDiagnosticStorage(IMetadataProvider metadataProvider)
        {
            MetadataSet targetMetadataSet;
            if (!metadataProvider.TryGetMetadata<MetadataOrderValidationIdentity>(out targetMetadataSet))
            {
                throw new InvalidOperationException("Can't get order validation metadata");
            }
            
            _sessionsCounterSetInstance = _sessionsCounterSet.CreateInstance("Default");
            _sessionsCounterSetInstance[Counters.Counters.Sessions.ActiveCount].RawValue = 0;
            _sessionsCounterSetInstance[Counters.Counters.Sessions.TotalCount].RawValue = 0;
            _sessionsCounterSetInstance[Counters.Counters.Sessions.TotalTimeSec].RawValue = 0;
            _sessionsCounterSetInstance[Counters.Counters.Sessions.TotalOrdersCount].RawValue = 0;
            _sessionsCounterSetInstance[Counters.Counters.Sessions.AvgValidationRateOrdersPerSec].RawValue = 0;

            foreach (var groupMetadata in targetMetadataSet.Metadata.Values.OfType<OrderValidationRuleGroupMetadata>())
            {
                var ruleGroupsCounterSetInstance = _ruleGroupsCounterSet.CreateInstance(groupMetadata.RuleGroup.ToString());
                _ruleGroupsCounterSetInstances.Add(groupMetadata.RuleGroup, ruleGroupsCounterSetInstance);

                ruleGroupsCounterSetInstance[Counters.Counters.RuleGroups.TotalTimeSec].RawValue = 0;
                ruleGroupsCounterSetInstance[Counters.Counters.RuleGroups.TotalOrdersCount].RawValue = 0;
                ruleGroupsCounterSetInstance[Counters.Counters.RuleGroups.ValidationResultsCacheUtilizationPercentage].RawValue = 0;
                ruleGroupsCounterSetInstance[Counters.Counters.RuleGroups.AvgValidationRateOrdersPerSec].RawValue = 0;
                ruleGroupsCounterSetInstance[Counters.Counters.RuleGroups.AvgConsumedTimePercentage].RawValue = 0;
            }

            foreach (var ruleMetadata in targetMetadataSet.Metadata.Values.OfType<OrderValidationRuleMetadata>())
            {
                var rulesCounterSetInstances = _rulesCounterSet.CreateInstance(ruleMetadata.RuleType.Name);
                _rulesCounterSetInstances.Add(ruleMetadata.RuleType, rulesCounterSetInstances);

                rulesCounterSetInstances[Counters.Counters.Rules.TotalTimeSec].RawValue = 0;
                rulesCounterSetInstances[Counters.Counters.Rules.TotalOrdersCount].RawValue = 0;
                rulesCounterSetInstances[Counters.Counters.Rules.ValidationResultsCacheUtilizationPercentage].RawValue = 0;
                rulesCounterSetInstances[Counters.Counters.Rules.AvgValidationRateOrdersPerSec].RawValue = 0;
                rulesCounterSetInstances[Counters.Counters.Rules.AvgConsumedTimePercentage].RawValue = 0;
            }
        }

        CounterSetInstance<Counters.Counters.Sessions> IOrderValidationDiagnosticStorage.Session
        {
            get { return _sessionsCounterSetInstance; }
        }

        CounterSetInstance<Counters.Counters.RuleGroups> IOrderValidationDiagnosticStorage.this[OrderValidationRuleGroup ruleGroup]
        {
            get { return _ruleGroupsCounterSetInstances[ruleGroup]; }
        }

        CounterSetInstance<Counters.Counters.Rules> IOrderValidationDiagnosticStorage.this[Type ruleType]
        {
            get { return _rulesCounterSetInstances[ruleType]; }
        }
    }
}
