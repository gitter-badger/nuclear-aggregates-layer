using System;
using System.Diagnostics;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.DiagnosticStorage;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback
{
    public sealed class OrderValidationOperationFeedback : IOrderValidationOperationFeedback
    {
        private readonly IOrderValidationDiagnosticStorage _orderValidationDiagnosticStorage;
        private readonly ICommonLog _logger;

        private readonly Stopwatch _operationTime = new Stopwatch();
        private readonly Stopwatch _validationTime = new Stopwatch();
        private readonly Stopwatch _cachingTime = new Stopwatch();
        private readonly Stopwatch _groupTime = new Stopwatch();
        private readonly Stopwatch _ruleTime = new Stopwatch();

        private OperationIndicators _operationIndicators;

        public OrderValidationOperationFeedback(IOrderValidationDiagnosticStorage orderValidationDiagnosticStorage, ICommonLog logger)
        {
            _orderValidationDiagnosticStorage = orderValidationDiagnosticStorage;
            _logger = logger;
        }

        void IOrderValidationOperationFeedback.OperationStarted(ValidationParams validationParams)
        {
            _logger.Info("Starting orders validation. " + validationParams);

            _operationIndicators = new OperationIndicators(validationParams);

            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.TotalCount].Increment();
            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.ActiveCount].Increment();

            _operationTime.Restart();
        }

        void IOrderValidationOperationFeedback.OperationFailed(Exception exception)
        {
            _operationTime.Stop();
            _operationIndicators.OperationExecutionTimeSec = _operationTime.Elapsed.TotalSeconds;
            _logger.ErrorFormat(exception, 
                                 "Orders validation failed and it takes {0:F2} sec. {1}",
                                 _operationIndicators.OperationExecutionTimeSec,
                                 _operationIndicators.ValidationParams);

            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.ActiveCount].Decrement();
        }

        void IOrderValidationOperationFeedback.OperationSucceeded()
        {
            _operationTime.Stop();
            _operationIndicators.OperationExecutionTimeSec = _operationTime.Elapsed.TotalSeconds;
            _logger.InfoFormat("Orders validation successfully finished and it takes {0:F2} sec. {1}",
                                 _operationIndicators.OperationExecutionTimeSec,
                                 _operationIndicators.ValidationParams);

            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.ActiveCount].Decrement();

            PublishIndicatorsForSucceededOperation();
        }

        void IOrderValidationOperationFeedback.ValidationStarted()
        {
            _logger.Info("Orders validation. Starting validation stage" + _operationIndicators.ValidationParams);
            _validationTime.Restart();
        }

        void IOrderValidationOperationFeedback.ValidationFailed(Exception exception)
        {
            _validationTime.Stop();
            _operationIndicators.ValidationExecutionTimeSec = _validationTime.Elapsed.TotalSeconds;
            _logger.ErrorFormat(exception,
                                  "Orders validation. Validation stage failed in {0:F2} sec. {1}",
                                  _operationIndicators.ValidationExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.ValidationSucceeded(int appropriateOrdersCount)
        {
            _validationTime.Stop();
            _operationIndicators.ValidationExecutionTimeSec = _validationTime.Elapsed.TotalSeconds;
            _operationIndicators.AppropriateOrdersCount = appropriateOrdersCount;
            _logger.InfoFormat("Orders validation. Validation stage successfully finished in {0:F2} sec. {1}",
                                  _operationIndicators.ValidationExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.CachingStarted()
        {
            _logger.Info("Orders validation. Starting caching stage" + _operationIndicators.ValidationParams);
            _cachingTime.Restart();
        }

        void IOrderValidationOperationFeedback.CachingFailed(Exception exception)
        {
            _cachingTime.Stop();
            _operationIndicators.CachingExecutionTimeSec = _cachingTime.Elapsed.TotalSeconds;
            _logger.ErrorFormat(exception,
                                  "Orders validation. Caching stage failed in {0:F2} sec. {1}",
                                  _operationIndicators.CachingExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.CachingSucceeded()
        {
            _cachingTime.Stop();
            _operationIndicators.CachingExecutionTimeSec = _cachingTime.Elapsed.TotalSeconds;
            _logger.InfoFormat("Orders validation. Caching stage successfully finished in {0:F2} sec. {1}",
                                  _operationIndicators.CachingExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.GroupStarted(OrderValidationRuleGroup ruleGroup)
        {
            _logger.InfoFormat("Orders validation. Group {0} started. {1}", ruleGroup, _operationIndicators.ValidationParams);
            _groupTime.Restart();
        }

        void IOrderValidationOperationFeedback.GroupFailed(OrderValidationRuleGroup ruleGroup, Exception exception)
        {
            _groupTime.Stop();
            var groupIndicators = new RuleGroupIndicators { ExecutionTimeSec = _groupTime.Elapsed.TotalSeconds };
            _operationIndicators.GroupIndicatorsRegistrar[ruleGroup] = groupIndicators;
            _logger.ErrorFormat(exception,
                                  "Orders validation. Group {0} failed in {1:F2} sec. {2}",
                                  ruleGroup,
                                  groupIndicators.ExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.GroupSucceeded(OrderValidationRuleGroup ruleGroup, int validatedOrdersCount)
        {
            _groupTime.Stop();
            var groupIndicators = new RuleGroupIndicators { ExecutionTimeSec = _groupTime.Elapsed.TotalSeconds, ValidatedOrdersCount = validatedOrdersCount };
            _operationIndicators.GroupIndicatorsRegistrar[ruleGroup] = groupIndicators;
            _logger.InfoFormat("Orders validation. Group {0} successfully finished in {1:F2} sec. {2}",
                                  ruleGroup,
                                  groupIndicators.ExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.RuleStarted(Type ruleType)
        {
            _logger.InfoFormat("Orders validation. Rule {0} started. {1}", ruleType.Name, _operationIndicators.ValidationParams);
            _ruleTime.Restart();
        }

        void IOrderValidationOperationFeedback.RuleFailed(Type ruleType, Exception exception)
        {
            _ruleTime.Stop();
            var ruleIndicators = new RuleIndicators { ExecutionTimeSec = _ruleTime.Elapsed.TotalSeconds };
            _operationIndicators.RuleIndicatorsRegistrar[ruleType] = ruleIndicators;
            _logger.ErrorFormat(exception,
                                  "Orders validation. Rule {0} failed in {1:F2} sec. {2}",
                                  ruleType.Name,
                                  ruleIndicators.ExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        void IOrderValidationOperationFeedback.RuleSucceeded(Type ruleType, int validatedOrdersCount)
        {
            _ruleTime.Stop();
            var ruleIndicators = new RuleIndicators { ExecutionTimeSec = _ruleTime.Elapsed.TotalSeconds, ValidatedOrdersCount = validatedOrdersCount };
            _operationIndicators.RuleIndicatorsRegistrar[ruleType] = ruleIndicators;
            _logger.InfoFormat("Orders validation. Rule {0} successfully finished in {1:F2} sec. {2}",
                                  ruleType.Name,
                                  ruleIndicators.ExecutionTimeSec,
                                  _operationIndicators.ValidationParams);
        }

        private void PublishIndicatorsForSucceededOperation()
        {
            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.TotalTimeSec].Value = (long)_operationIndicators.OperationExecutionTimeSec;
            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.TotalOrdersCount].IncrementBy(_operationIndicators.AppropriateOrdersCount);
            _orderValidationDiagnosticStorage.Session[Counters.Counters.Sessions.AvgValidationRateOrdersPerSec].Value = 
                (long)(_operationIndicators.AppropriateOrdersCount / _operationIndicators.OperationExecutionTimeSec);

            foreach (var groupIndicators in _operationIndicators.GroupIndicatorsRegistrar)
            {
                _orderValidationDiagnosticStorage[groupIndicators.Key][Counters.Counters.RuleGroups.TotalTimeSec].Value = (long)groupIndicators.Value.ExecutionTimeSec;
                _orderValidationDiagnosticStorage[groupIndicators.Key][Counters.Counters.RuleGroups.TotalOrdersCount].Value = groupIndicators.Value.ValidatedOrdersCount;
                _orderValidationDiagnosticStorage[groupIndicators.Key][Counters.Counters.RuleGroups.ValidationResultsCacheUtilizationPercentage].Value =
                    100 * (1 - (groupIndicators.Value.ValidatedOrdersCount / _operationIndicators.AppropriateOrdersCount));
                _orderValidationDiagnosticStorage[groupIndicators.Key][Counters.Counters.RuleGroups.AvgValidationRateOrdersPerSec].Value =
                     (long)(groupIndicators.Value.ValidatedOrdersCount / groupIndicators.Value.ExecutionTimeSec);
                _orderValidationDiagnosticStorage[groupIndicators.Key][Counters.Counters.RuleGroups.AvgConsumedTimePercentage].Value =
                    (long)(100 * groupIndicators.Value.ExecutionTimeSec / _operationIndicators.ValidationExecutionTimeSec);
            }

            foreach (var ruleIndicators in _operationIndicators.RuleIndicatorsRegistrar)
            {
                _orderValidationDiagnosticStorage[ruleIndicators.Key][Counters.Counters.Rules.TotalTimeSec].Value = (long)ruleIndicators.Value.ExecutionTimeSec;
                _orderValidationDiagnosticStorage[ruleIndicators.Key][Counters.Counters.Rules.TotalOrdersCount].Value = ruleIndicators.Value.ValidatedOrdersCount;
                _orderValidationDiagnosticStorage[ruleIndicators.Key][Counters.Counters.Rules.ValidationResultsCacheUtilizationPercentage].Value =
                    100 * (1 - (ruleIndicators.Value.ValidatedOrdersCount / _operationIndicators.AppropriateOrdersCount));
                _orderValidationDiagnosticStorage[ruleIndicators.Key][Counters.Counters.Rules.AvgValidationRateOrdersPerSec].Value =
                     (long)(ruleIndicators.Value.ValidatedOrdersCount / ruleIndicators.Value.ExecutionTimeSec);
                _orderValidationDiagnosticStorage[ruleIndicators.Key][Counters.Counters.Rules.AvgConsumedTimePercentage].Value =
                    (long)(100 * ruleIndicators.Value.ExecutionTimeSec / _operationIndicators.ValidationExecutionTimeSec);
            }
        }
    }
}