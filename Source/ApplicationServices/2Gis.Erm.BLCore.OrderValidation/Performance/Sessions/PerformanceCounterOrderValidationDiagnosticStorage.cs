using System;
using System.Collections.Generic;
using System.Linq;

using DevUtils.ETWIMBA.Diagnostics.Counters;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions
{
    public sealed partial class PerformanceCounterOrderValidationDiagnosticStorage : IOrderValidationDiagnosticStorage
    {
        private readonly CounterSet<Counters.Counters.Sessions> _sessionsCounterSet = new CounterSet<Counters.Counters.Sessions>();
        private readonly CounterSet<Counters.Counters.RuleGroups> _ruleGroupsCounterSet = new CounterSet<Counters.Counters.RuleGroups>();
        private readonly CounterSet<Counters.Counters.Rules> _rulesCounterSet = new CounterSet<Counters.Counters.Rules>();

        private readonly CounterSetInstance<Counters.Counters.Sessions> _sessionsCounterSetInstance;
        private readonly Dictionary<OrderValidationRuleGroup, CounterSetInstance<Counters.Counters.RuleGroups>> _ruleGroupsCounterSetInstances;
        private readonly Dictionary<Type, CounterSetInstance<Counters.Counters.Rules>> _rulesCounterSetInstances;

        public PerformanceCounterOrderValidationDiagnosticStorage(IMetadataProvider metadataProvider)
        {
            _sessionsCounterSetInstance = _sessionsCounterSet.CreateInstance("Default");

            MetadataSet targetMetadataSet;
            if (!metadataProvider.TryGetMetadata<MetadataOrderValidationIdentity>(out targetMetadataSet))
            {
                throw new InvalidOperationException("Can't get order validation metadata");
            }

            _ruleGroupsCounterSetInstances = targetMetadataSet.Metadata.OfType<OrderValidationRuleGroupMetadata>()
                                                              .ToDictionary(
                                                                  m => m.RuleGroup, 
                                                                  m => _ruleGroupsCounterSet.CreateInstance(m.RuleGroup.ToString()));

            _rulesCounterSetInstances = targetMetadataSet.Metadata.OfType<OrderValidationRuleMetadata>()
                                                              .ToDictionary(
                                                                  m => m.RuleType,
                                                                  m => _rulesCounterSet.CreateInstance(m.RuleType.Name));
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
