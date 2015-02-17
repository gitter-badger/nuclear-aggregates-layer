using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Settings;
using DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions.Feedback;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ValidateOrdersOperationService : IValidateOrdersOperationService
    {
        private readonly IOrderValidationCachingSettings _orderValidationCachingSettings;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderValidationRuleProvider _orderValidationRuleProvider;
        private readonly IOrderValidationPredicateFactory _orderValidationPredicateFactory;
        private readonly IAttachValidResultToCacheAggregateService _attachValidResultToCacheAggregateService;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IOrderValidationOperationFeedback _operationFeedback;
        private readonly ICommonLog _logger;

        public ValidateOrdersOperationService(
            IOrderValidationCachingSettings orderValidationCachingSettings,
            IOrderReadModel orderReadModel,
            IOrderValidationRuleProvider orderValidationRuleProvider,
            IOrderValidationPredicateFactory orderValidationPredicateFactory,
            IAttachValidResultToCacheAggregateService attachValidResultToCacheAggregateService,
            IUseCaseTuner useCaseTuner,
            IOperationScopeFactory scopeFactory,
            IOrderValidationOperationFeedback operationFeedback,
            ICommonLog logger)
        {
            _orderValidationCachingSettings = orderValidationCachingSettings;
            _orderReadModel = orderReadModel;
            _orderValidationRuleProvider = orderValidationRuleProvider;
            _orderValidationPredicateFactory = orderValidationPredicateFactory;
            _attachValidResultToCacheAggregateService = attachValidResultToCacheAggregateService;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
            _operationFeedback = operationFeedback;
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

            var scope = CreateOperationScope();

            var validationType = newOrderState == OrderState.NotSet || currentOrderState == OrderState.OnRegistration
                                     ? ValidationType.SingleOrderOnRegistration
                                     : ValidationType.SingleOrderOnStateChanging;

            var validationParams = new SingleOrderValidationParams(scope.Id, validationType)
                                       {
                                           OrderId = orderId,
                                           CurrentOrderState = currentOrderState,
                                           NewOrderState = newOrderState,
                                           Period = new TimePeriod(order.BeginDistributionDate, order.BeginDistributionDate.AddMonths(1).AddSeconds(-1))
                                       };

            return Validate(validationParams, scope);
        }

        ValidationResult IValidateOrdersOperationService.Validate(
            ValidationType validationType, 
            long organizationUnitId, 
            TimePeriod period, 
            long? ownerCode,
            bool includeOwnerDescendants)
        {
            var scope = CreateOperationScope();

            var validationParams = new MassOrdersValidationParams(scope.Id, validationType)
            {
                OrganizationUnitId = organizationUnitId,
                Period = period,
                OwnerId = ownerCode,
                IncludeOwnerDescendants = includeOwnerDescendants
            };

            return Validate(validationParams, scope);
        }

        [Obsolete("После выпиливания UseLegacyCachingMode - избавиться от данного метода, заменив на прямое использование _scopeFactory")]
        private IOperationScope CreateOperationScope()
        {
            return !_orderValidationCachingSettings.UseLegacyCachingMode
                       ? _scopeFactory.CreateNonCoupled<ValidateOrdersIdentity>()
                       : new NullOperationScope(true, ValidateOrdersIdentity.Instance.NonCoupled());
        }

        private ValidationResult Validate(ValidationParams validationParams, IOperationScope validationOperationScope)
        {
            _useCaseTuner.AlterDuration<ValidateOrdersOperationService>();

            _operationFeedback.OperationStarted(validationParams);
            
            var resultsContainer = new ValidationResultsContainer();

            try
            {
                using (validationOperationScope)
                {
                    var ruleGroupContainers = _orderValidationRuleProvider.GetAppropriateRules(validationParams.Type);

                    try
                    {
                        int appropriateOrdersCount;
                        _operationFeedback.ValidationStarted();
                        ValidateOrders(validationParams, ruleGroupContainers, resultsContainer, out appropriateOrdersCount);
                        _operationFeedback.ValidationSucceeded(appropriateOrdersCount);
                    }
                    catch (Exception ex)
                    {
                        _operationFeedback.ValidationFailed(ex);
                        throw;
                    }

                    try
                    {
                        _operationFeedback.CachingStarted();
                        AttachResultsToCache(validationParams, ruleGroupContainers, resultsContainer);
                        _operationFeedback.CachingSucceeded();
                    }
                    catch (Exception ex)
                    {
                        _operationFeedback.CachingFailed(ex);
                        throw;
                    }

                    validationOperationScope.Complete();
                }

                _operationFeedback.OperationSucceeded();
            }
            catch (Exception ex)
            {
                _operationFeedback.OperationFailed(ex);
                throw;
            }

            return resultsContainer.ToValidationResult();
        }

        private void ValidateOrders(
            ValidationParams validationParams, 
            IEnumerable<OrderValidationRulesContainer> ruleGroupContainers, 
            ValidationResultsContainer resultsContainer, 
            out int appropriateOrdersCount)
        {
            var filterPredicate = _orderValidationPredicateFactory.CreatePredicate(validationParams);
            appropriateOrdersCount = _orderReadModel.GetOrdersCurrentVersions(filterPredicate.GetCombinedPredicate()).Count;
            if (appropriateOrdersCount == 0)
            {
                _logger.Info("Orders validation. Skip validation. No orders to validate. " + validationParams);
                return;
            }

            foreach (var ruleGroupContainer in ruleGroupContainers)
            {
                if (!ruleGroupContainer.RuleDescriptors.Any())
                {
                    _logger.InfoFormat("Orders validation. Group {0} skipped. Appropriate rules count: 0. {1}", ruleGroupContainer.Group, validationParams);
                    continue;
                }
            
                try
                {
                    int ordersCount;
                    _operationFeedback.GroupStarted(ruleGroupContainer.Group);
                    ValidateByRuleGroup(validationParams, filterPredicate, ruleGroupContainer, resultsContainer, out ordersCount);
                    _operationFeedback.GroupSucceeded(ruleGroupContainer.Group, ordersCount);
                }
                catch (Exception ex)
                {
                    _operationFeedback.GroupFailed(ruleGroupContainer.Group, ex);
                    throw;
                }
            }
        }

        private void ValidateByRuleGroup(
            ValidationParams validationParams, 
            OrderValidationPredicate filterPredicate, 
            OrderValidationRulesContainer ruleGroupContainer, 
            ValidationResultsContainer resultsContainer,
            out int ordersCount)
        {
            var combinedPredicate = CreateValidationPredicate(filterPredicate, ruleGroupContainer);
            var ordersForValidationWitVersions = _orderReadModel.GetOrdersCurrentVersions(combinedPredicate.GetCombinedPredicate());
            ordersCount = ordersForValidationWitVersions.Count;
            if (ordersCount == 0)
            {
                _logger.InfoFormat("Orders validation. Group {0} skipped. No orders to validate. {1}", ruleGroupContainer.Group, validationParams);
                return;
            }

            _logger.InfoFormat("Orders validation. Group {0}. Rules in group actual count: {1}. {2}",
                                 ruleGroupContainer.Group,
                                 ruleGroupContainer.RuleDescriptors.Count,
                                 validationParams);

            foreach (var ruleDescriptor in ruleGroupContainer.RuleDescriptors)
            {
                var validatorDescriptor = new ValidatorDescriptor(ruleGroupContainer.Group, ruleDescriptor.RuleCode);
                resultsContainer.AttachTargets(validatorDescriptor, ordersForValidationWitVersions);

                // TODO {all, 20.10.2014}: выпилить legacyTransaction, после исключения проблем с нагрузкой на tempdb варианта кэша проверок использующего версии заказов
                TransactionScope legacyTransaction = !_orderValidationCachingSettings.UseLegacyCachingMode
                                                         ? null
                                                         : new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default);
                
                try
                {
                    _operationFeedback.RuleStarted(ruleDescriptor.Rule.GetType());
                    var results = ruleDescriptor.Rule.Validate(validationParams, combinedPredicate, resultsContainer);
                    resultsContainer.AttachResults(validatorDescriptor, results);
                    
                    if (legacyTransaction != null)
                    {
                        legacyTransaction.Complete();
                    }

                    _operationFeedback.RuleSucceeded(ruleDescriptor.Rule.GetType(), ordersCount);
                }
                catch (Exception ex)
                {
                    _operationFeedback.RuleFailed(ruleDescriptor.Rule.GetType(), ex);

                    // TODO {all, 01.10.2014}: оценить так ли необходимо фейлить всю сессию проверки, или можно обойтись только этим rule
                    throw;
                }
                finally
                {
                    if (legacyTransaction != null)
                    {
                        legacyTransaction.Dispose();
                    }
                }
            }
        }

        private OrderValidationPredicate CreateValidationPredicate(OrderValidationPredicate filterPredicate, OrderValidationRulesContainer ruleGroupContainer)
        {
            bool useCachedResultsDisabled = !ruleGroupContainer.UseCaching || ruleGroupContainer.RuleDescriptors.Any(rd => !rd.UseCaching);

            return
                useCachedResultsDisabled
                    ? filterPredicate
                    : !_orderValidationCachingSettings.UseLegacyCachingMode
                        ? new OrderValidationPredicate(filterPredicate.GeneralPart,
                                                       filterPredicate.OrgUnitPart,
                                                       x => !x.OrderValidationCacheEntries.Any(y => y.ValidatorId == (int)ruleGroupContainer.Group && y.ValidVersion == x.Timestamp))
                        : new OrderValidationPredicate(filterPredicate.GeneralPart,
                                                       filterPredicate.OrgUnitPart,
                                                       x => !x.OrderValidationResults
                                                                        .Where(y => y.OrderValidationGroupId == (int)ruleGroupContainer.Group)
                                                                        .OrderByDescending(y => y.Id)
                                                                        .Select(y => y.IsValid)
                                                                        .FirstOrDefault());
        }

        private void AttachResultsToCache(
            ValidationParams validationParams, 
            IEnumerable<OrderValidationRulesContainer> ruleGroupContainers, 
            IValidationResultsBrowser validationResultsBrowser)
        {
            var cachableRuleGroups = ruleGroupContainers.Where(rg => rg.UseCaching && rg.AllRulesScheduled).Select(x => x.Group);

            var cachableValidatorsByGroups =
                    validationResultsBrowser.ScheduledValidatorsSequence
                                            .Where(v => cachableRuleGroups.Contains(v.RuleGroup))
                                            .GroupBy(v => v.RuleGroup);

            var validResultsForCaching = new List<OrderValidationCacheEntry>();

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

                validResultsForCaching.AddRange(validatorsGroupTargetOrders.Select(x => new OrderValidationCacheEntry
                                                                                            {
                                                                                                OrderId = x.Key,
                                                                                                ValidatorId = (int)validatorsGroup.Key,
                                                                                                ValidVersion = x.Value,
                                                                                                OperationId = validationParams.Token
                                                                                            }));
            }

            _attachValidResultToCacheAggregateService.Attach(validResultsForCaching);
        }
    }
}
