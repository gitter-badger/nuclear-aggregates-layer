using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback
{
    internal sealed class OperationIndicators
    {
        private readonly ValidationParams _validationParams;
        private readonly Dictionary<OrderValidationRuleGroup, RuleGroupIndicators> _groupIndicatorsRegistrar = new Dictionary<OrderValidationRuleGroup, RuleGroupIndicators>();
        private readonly Dictionary<Type, RuleIndicators> _ruleIndicatorsRegistrar = new Dictionary<Type, RuleIndicators>();

        public OperationIndicators(ValidationParams validationParams)
        {
            _validationParams = validationParams;
        }

        public Dictionary<OrderValidationRuleGroup, RuleGroupIndicators> GroupIndicatorsRegistrar
        {
            get { return _groupIndicatorsRegistrar; }
        }

        public Dictionary<Type, RuleIndicators> RuleIndicatorsRegistrar
        {
            get { return _ruleIndicatorsRegistrar; }
        }

        public ValidationParams ValidationParams
        {
            get { return _validationParams; }
        }

        public int AppropriateOrdersCount { get; set; }
        public double OperationExecutionTimeSec { get; set; }
        public double ValidationExecutionTimeSec { get; set; }
        public double CachingExecutionTimeSec { get; set; }
    }
}