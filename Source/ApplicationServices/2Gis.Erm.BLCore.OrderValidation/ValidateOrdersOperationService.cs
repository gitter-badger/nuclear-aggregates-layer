using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ValidateOrdersOperationService : IValidateOrdersOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderValidationRuleProvider _orderValidationRuleProvider;
        private readonly IOrderValidationPredicateFactory _orderValidationPredicateFactory;
        private readonly IAttachValidResultToCacheAggregateService _attachValidResultToCacheAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public ValidateOrdersOperationService(
            IOrderReadModel orderReadModel,
            IOrderValidationRuleProvider orderValidationRuleProvider,
            IOrderValidationPredicateFactory orderValidationPredicateFactory,
            IAttachValidResultToCacheAggregateService attachValidResultToCacheAggregateService,
            IUseCaseTuner useCaseTuner,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _orderReadModel = orderReadModel;
            _orderValidationRuleProvider = orderValidationRuleProvider;
            _orderValidationPredicateFactory = orderValidationPredicateFactory;
            _attachValidResultToCacheAggregateService = attachValidResultToCacheAggregateService;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        ValidationResult IValidateOrdersOperationService.Validate(long orderId)
        {
            return ((IValidateOrdersOperationService)this).Validate(orderId, OrderState.NotSet);
        }

        ValidationResult IValidateOrdersOperationService.Validate(long orderId, OrderState newOrderState)
        {
            var order = _orderReadModel.GetOrderUnsecure(orderId);
            var currentOrderState = (OrderState)order.WorkflowStepId;
            var validationParams = new SingleOrderValidationParams(newOrderState == OrderState.NotSet || currentOrderState == OrderState.OnRegistration
                                                                   ? ValidationType.SingleOrderOnRegistration
                                                                   : ValidationType.SingleOrderOnStateChanging)
            {
                OrderId = orderId,
                CurrentOrderState = currentOrderState,
                NewOrderState = newOrderState,
                Period = new TimePeriod(order.BeginDistributionDate, order.BeginDistributionDate.AddMonths(1).AddSeconds(-1))
            };

            return Validate(validationParams);
        }

        ValidationResult IValidateOrdersOperationService.Validate(
            ValidationType validationType, 
            long organizationUnitId, 
            TimePeriod period, 
            long? ownerCode, 
            bool includeOwnerDescendants)
        {
            var validationParams = new MassOrdersValidationParams(validationType)
            {
                OrganizationUnitId = organizationUnitId,
                Period = period,
                OwnerId = ownerCode,
                IncludeOwnerDescendants = includeOwnerDescendants
            };

            return Validate(validationParams);
        }

        private ValidationResult Validate(ValidationParams validationParams)
        {
            _useCaseTuner.AlterDuration<ValidateOrdersOperationService>();

            _logger.InfoEx("Starting order validation. " + validationParams);
            
            var resultsContainer = new ValidationResultsContainer();
            var ruleGroupContainers = _orderValidationRuleProvider.GetAppropriateRules(validationParams.Type);
            using (var scope = _scopeFactory.CreateNonCoupled<ValidateOrdersIdentity>())
            {
                ValidateOrders(validationParams, ruleGroupContainers, resultsContainer);
                AttachResultsToCache(validationParams, ruleGroupContainers, resultsContainer);
                
                scope.Complete();
            }

            return resultsContainer.ToValidationResult();
        }

        private void ValidateOrders(ValidationParams validationParams, IEnumerable<OrderValidationRulesContianer> ruleGroupContainers, ValidationResultsContainer resultsContainer)
        {
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var ruleGroupContainer in ruleGroupContainers)
            {
                if (!ruleGroupContainer.RuleDescriptors.Any())
                {
                    _logger.InfoFormatEx("Skip validation by group [{0}]. Appropriate rules count: 0. {1}", ruleGroupContainer.Group, validationParams);
                    continue;
                }

                ValidateByRuleGroup(validationParams, ruleGroupContainer, resultsContainer);
            }

            stopwatch.Stop();
            _logger.InfoFormatEx("Validation completed in {0:F2} sec. {1}", stopwatch.ElapsedMilliseconds / 1000D, validationParams);
        }

        private void ValidateByRuleGroup(ValidationParams validationParams, OrderValidationRulesContianer ruleGroupContainer, ValidationResultsContainer resultsContainer)
        {
            bool useCachedResultsDisabled = !ruleGroupContainer.UseCaching || ruleGroupContainer.RuleDescriptors.Any(rd => !rd.UseCaching);

            var filterPredicate = _orderValidationPredicateFactory.CreatePredicate(validationParams);
            
            // FIXME {i.maslennikov, 25.09.2014}: проверить корректность сравнения версий в генерируемом SQL
            var combinedPredicate = 
                !useCachedResultsDisabled 
                    ? new OrderValidationPredicate(filterPredicate.GeneralPart,
                                                   filterPredicate.OrgUnitPart,
                                                   x => !x.OrderValidationResults.Any(y => y.ValidatorId == (int)ruleGroupContainer.Group && y.ValidVersion == x.Timestamp)) 
                    : filterPredicate;

            var ordersForValidationWitVersions = _orderReadModel.GetOrdersCurrentVersions(combinedPredicate.GetCombinedPredicate());
            if (ordersForValidationWitVersions.Count == 0)
            {
                _logger.InfoFormatEx("Skipping validating orders by group [{0}]. No orders to validate. {1}", ruleGroupContainer.Group, validationParams);
                return;
            }

            _logger.InfoFormatEx("Validating orders by group [{0}]. Rules in group actual count: [{1}]. Orders to validate count: [{2}]. {3}",
                                 ruleGroupContainer.Group,
                                 ruleGroupContainer.RuleDescriptors.Count,
                                 ordersForValidationWitVersions.Count,
                                 validationParams);

            var stopwatch = Stopwatch.StartNew();

            foreach (var ruleDescriptor in ruleGroupContainer.RuleDescriptors)
            {
                var validatorDescriptor = new ValidatorDescriptor(ruleGroupContainer.Group, ruleDescriptor.RuleCode);
                resultsContainer.AttachTargets(validatorDescriptor, ordersForValidationWitVersions);
                var results = ValidateByRule(ruleDescriptor.Rule, validationParams, combinedPredicate, resultsContainer);
                resultsContainer.AttachResults(validatorDescriptor, results);
            }

            stopwatch.Stop();
            _logger.InfoFormatEx(
                "Validating orders by group [{0}] completed in {1:F2} sec. {2}", 
                ruleGroupContainer.Group,
                stopwatch.ElapsedMilliseconds / 1000D,
                validationParams);
        }

        private IEnumerable<OrderValidationMessage> ValidateByRule(
            IOrderValidationRule rule,
            ValidationParams validationParams,
            OrderValidationPredicate combinedPredicate,
            ValidationResultsContainer resultsContainer)
        {
            IEnumerable<OrderValidationMessage> ruleMessages;
            
            var stopwatch = Stopwatch.StartNew();

            try
            {
                ruleMessages = rule.Validate(validationParams, combinedPredicate, resultsContainer);
                stopwatch.Stop();
                _logger.InfoFormatEx("Rule '{0}' executed in {1:F2} sec. {2}",
                                     rule.GetType().Name,
                                     stopwatch.ElapsedMilliseconds / 1000D,
                                     validationParams);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.ErrorFormatEx(ex, 
                                     "Rule '{0}' failed after {1:F2} sec. {2}",
                                     rule.GetType().Name,
                                     stopwatch.ElapsedMilliseconds / 1000D,
                                     validationParams);

                // TODO {all, 01.10.2014}: оценить так ли необходимо фейлить всю сессию проверки, или можно обойтись только этим rule
                throw;
            }

            return ruleMessages;
        }

        private void AttachResultsToCache(
            ValidationParams validationParams, 
            IEnumerable<OrderValidationRulesContianer> ruleGroupContainers, 
            IValidationResultsBrowser validationResultsBrowser)
        {
            var stopwatch = Stopwatch.StartNew();

            var cachableRuleGroups = ruleGroupContainers.Where(rg => rg.UseCaching && rg.AllRulesScheduled).Select(x => x.Group);

            var cachableValidatorsByGroups =
                    validationResultsBrowser.ScheduledValidatorsSequence
                                            .Where(v => cachableRuleGroups.Contains(v.RuleGroup))
                                            .GroupBy(v => v.RuleGroup);

            var validResultsForCaching = new List<OrderValidationResult>();

            foreach (var validatorsGroup in cachableValidatorsByGroups)
            {
                bool completelyValidatedGroup = true;
                var invalidOrderIds = new HashSet<long>();
                var validatorsGroupTargetOrders = new Dictionary<long, byte[]>();

                foreach (var validator in validatorsGroup)
                {
                    IReadOnlyDictionary<long, byte[]> validatorTargetOrders;
                    IReadOnlyList<OrderValidationMessage> validatorResults;
                    if (!validationResultsBrowser.TryGetValidatorReport(validator, out validatorTargetOrders, out validatorResults))
                    {
                        throw new InvalidOperationException("Can't get report for validator. " + validator);
                    }

                    if (validatorResults == null)
                    {   // т.е. проверка через rule была запущена, однако, результатов проверки нет в контейнере, 
                        // возможно, во время работы возникло исключение, т.е. фактически проверка не выполнена
                        completelyValidatedGroup = false;
                        break;
                    }

                    foreach (var result in validatorResults)
                    {
                        if (!result.IsOrderSpecific() || !result.IsInvalid())
                        {
                            continue;
                        }

                        invalidOrderIds.Add(result.OrderId);
                    }

                    foreach (var targetOrderBucket in validatorTargetOrders)
                    {
                        byte[] candidateVersion;
                        if (validatorsGroupTargetOrders.TryGetValue(targetOrderBucket.Key, out candidateVersion))
                        {
                            // TODO {all, 04.10.2014}: пока проверка отключена, чтобы не влияла на performance, если основанная на версиях инфрастуктура кэширования проявит себя нормально, скоре всего нужно будет включить проверку
                            /*if (!targetOrderBucket.Value.SameAs(candidateVersion))
                            {   // почему-то разные проверки в рамках одной группы проверили разные версии заказа - чего при транзакционной работе быть недолжно
                                throw new InvalidOperationException("Validators in the same group " + validator.RuleGroup + " use different versions of order " + targetOrderBucket.Key + " that is unacceptable behaviour");
                            }*/
                        }
                        else
                        {
                            validatorsGroupTargetOrders.Add(targetOrderBucket.Key, targetOrderBucket.Value);
                        }
                    }
                }

                if (!completelyValidatedGroup)
                {
                    continue;
                }

                foreach (var invalidOrderId in invalidOrderIds)
                {
                    validatorsGroupTargetOrders.Remove(invalidOrderId);
                }

                if (validatorsGroupTargetOrders.Any())
                {
                    var invalidRelatedOrdersMap = _orderReadModel.GetRelatedOrdersByFirm(invalidOrderIds);
                    foreach (var invalidRelatedOrderIds in invalidRelatedOrdersMap.Values)
                    {
                        foreach (var invalidRelatedOrderId in invalidRelatedOrderIds)
                        {
                            validatorsGroupTargetOrders.Remove(invalidRelatedOrderId);
                        }
                    }
                }

                validResultsForCaching.AddRange(validatorsGroupTargetOrders.Select(x => new OrderValidationResult
                                                                                            {
                                                                                                OrderId = x.Key,
                                                                                                ValidatorId = (int)validatorsGroup.Key,
                                                                                                ValidVersion = x.Value
                                                                                            }));
            }

            _attachValidResultToCacheAggregateService.Attach(validResultsForCaching);

            stopwatch.Stop();
            _logger.InfoFormatEx("Validation results cached in {0:F2} sec. {1}", stopwatch.ElapsedMilliseconds / 1000D, validationParams);
        }
    }
}
