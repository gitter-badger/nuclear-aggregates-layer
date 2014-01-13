using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class OrderValidationService : IOrderValidationService, IOrderValidationResultsResetter
    {
        #region validation types map

        private static readonly Dictionary<int, Type> RuleCodeMap = new Dictionary<int, Type>
            {
                { 1, typeof(BargainOutOfDateOrderValidationRule) },
                { 2, typeof(CouponPeriodOrderValidationRule) },
                { 3, typeof(AccountExistsOrderValidationRule) },
                { 6, typeof(AssociatedAndDeniedPricePositionsOrderValidationRule) },
                { 7, typeof(BillsSumsOrderValidationRule) },
                { 8, typeof(CategoriesLinkedToDestOrgUnitOrderValidationRule) },
                { 9, typeof(DistributionDatesOrderValidationRule) },
                { 10, typeof(FirmBelongsToOrdersDestOrgUnitOrderValidationRule) },
                { 11, typeof(FirmsOrderValidationRule) },
                { 12, typeof(LinkingObjectsOrderValidationRule) },
                { 13, typeof(LockNoExistsOrderValidationRule) },
                { 14, typeof(OrderHasAtLeastOnePositionOrderValidationRule) },
                { 15, typeof(OrderPositionsRefereceCurrentPriceListOrderValidationRule) },
                { 16, typeof(OrdersAndBargainsScansExistOrderValidationRule) },
                { 17, typeof(ReleaseNotExistsOrderValidationRule) },
                { 18, typeof(RequiredFieldsNotEmptyOrderValidationRule) },
                { 20, typeof(BalanceOrderValidationRule) },
                { 22, typeof(ValidateAdvertisementsOrderValidationRule) },
                { 23, typeof(LegalPersonProfilesOrderValidationRule) },
                { 24, typeof(WarrantyEndDateOrderValidationRule) },
                { 25, typeof(BargainEndDateOrderValidationRule) },
                { 26, typeof(AdvertisementAmountOrderValidationRule) },
                { 27, typeof(ContactDoesntContainSponsoredLinkOrderValidationRule) },
                { 29, typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule) },
                { 30, typeof(IsThereAdvantageousPurchasesRubricOrderValidationRule) },
                { 31, typeof(AdvertisementForCategoryAmountOrderValidationRule) },
                { 32, typeof(CategoriesForFirmAmountOrderValidationRule) },
                { 33, typeof(IsPositionSupportedByExportOrderValidationRule) },
                { 34, typeof(IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule) },
                { 35, typeof(CouponIsUniqueForFirmOrderValidationRule) },
                { 36, typeof(SelfAdvertisementOrderValidationRule) },
                { 37, typeof(AdditionalAdvertisementsOrderValidationRule) },
                { 38, typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule) },
                { 39, typeof(PlatformTypeOrderValidationRule) },
                { 40, typeof(DefaultThemeMustBeSpecifiedValidationRule) },
                { 41, typeof(DefaultThemeMustContainOnlySelfAdvValidationRule) },
                { 42, typeof(ThemePeriodOverlapsOrderPeriodValidationRule) },
                { 43, typeof(ThemeCategoriesValidationRule) },
                { 44, typeof(ThemePositionCountValidationRule) },
                { 47, typeof(DummyAdvertisementOrderValidationRule) },
                { 48, typeof(RegionalApiAdvertisementsOrderValidationRule) },
            };

        private static readonly Type[] CommonRules =
            {
                typeof(AssociatedAndDeniedPricePositionsOrderValidationRule),
                typeof(FirmBelongsToOrdersDestOrgUnitOrderValidationRule),
                typeof(ValidateAdvertisementsOrderValidationRule),
                typeof(CouponPeriodOrderValidationRule),
                typeof(SelfAdvertisementOrderValidationRule),
                typeof(IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule),
                typeof(AdditionalAdvertisementsOrderValidationRule),
                typeof(PlatformTypeOrderValidationRule),
                typeof(IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule),
                typeof(IsPositionSupportedByExportOrderValidationRule),
                typeof(AdvertisementForCategoryAmountOrderValidationRule),
                typeof(ThemePositionCountValidationRule),
                typeof(ThemePeriodOverlapsOrderPeriodValidationRule),
                typeof(DefaultThemeMustContainOnlySelfAdvValidationRule),
                typeof(DummyAdvertisementOrderValidationRule),
                typeof(CouponIsUniqueForFirmOrderValidationRule),
                typeof(RegionalApiAdvertisementsOrderValidationRule)
            };

        private static readonly Type[] NonManualRules =
            {
                typeof(CategoriesLinkedToDestOrgUnitOrderValidationRule),
                typeof(DistributionDatesOrderValidationRule),
                typeof(FirmsOrderValidationRule),
                typeof(LinkingObjectsOrderValidationRule),
                typeof(OrderHasAtLeastOnePositionOrderValidationRule),
                typeof(RequiredFieldsNotEmptyOrderValidationRule),
                typeof(AdvertisementAmountOrderValidationRule)
            };

        private static readonly Dictionary<ValidationType, Type[]> ValidationTypesMap = new Dictionary<ValidationType, Type[]>
            {
                {
                    ValidationType.SingleOrderOnRegistration,

                    CommonRules
                        .Concat(NonManualRules)
                        .Concat(new[]
                            {
                                typeof(BargainOutOfDateOrderValidationRule),
                                typeof(BillsSumsOrderValidationRule),
                                typeof(LegalPersonProfilesOrderValidationRule),
                                typeof(WarrantyEndDateOrderValidationRule),
                                typeof(BargainEndDateOrderValidationRule),
                                typeof(OrderPositionsRefereceCurrentPriceListOrderValidationRule),
                                typeof(OrdersAndBargainsScansExistOrderValidationRule),
                                typeof(ReleaseNotExistsOrderValidationRule),
                                typeof(ContactDoesntContainSponsoredLinkOrderValidationRule),
                                typeof(CategoriesForFirmAmountOrderValidationRule),
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule),
                                typeof(IsThereAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                },
                {
                    ValidationType.PreReleaseBeta,

                    CommonRules
                        .Concat(NonManualRules)
                        .Concat(new[]
                            {
                                typeof(AccountExistsOrderValidationRule),
                                typeof(LockNoExistsOrderValidationRule),
                                typeof(ThemeCategoriesValidationRule),
                                typeof(DefaultThemeMustBeSpecifiedValidationRule),
                                typeof(CategoriesForFirmAmountOrderValidationRule),
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule),
                                typeof(IsThereAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                },
                {
                    ValidationType.PreReleaseFinal,

                    CommonRules
                        .Concat(NonManualRules)
                        .Concat(new[]
                            {
                                typeof(BalanceOrderValidationRule),
                                typeof(AccountExistsOrderValidationRule),
                                typeof(LockNoExistsOrderValidationRule),
                                typeof(ThemeCategoriesValidationRule),
                                typeof(DefaultThemeMustBeSpecifiedValidationRule),
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                },
                {
                    ValidationType.ManualReport,

                    CommonRules
                        .Concat(new[]
                            {
                                typeof(ThemeCategoriesValidationRule),
                                typeof(DefaultThemeMustBeSpecifiedValidationRule),
                                typeof(CategoriesForFirmAmountOrderValidationRule),
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule),
                                typeof(IsThereAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                },
                {
                    ValidationType.ManualReportWithAccountsCheck,

                    CommonRules
                        .Concat(new[]
                            {
                                typeof(BalanceOrderValidationRule),
                                typeof(ThemeCategoriesValidationRule),
                                typeof(DefaultThemeMustBeSpecifiedValidationRule),
                                typeof(CategoriesForFirmAmountOrderValidationRule),
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule),
                                typeof(IsThereAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                },
            };

        #endregion

        private readonly ICommonLog _logger;
        private readonly IFinder _finder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderValidationRepository _orderValidationRepository;
        private readonly IEnumerable<IOrderValidationRule> _allValidationRules;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderValidationService(
            ICommonLog logger,
            IFinder finder,
            IUnitOfWork unitOfWork,
            IOrderValidationRepository orderValidationRepository,
            // ReSharper disable ParameterTypeCanBeEnumerable.Local
            IOrderValidationRule[] allValidationRules,
            // ReSharper restore ParameterTypeCanBeEnumerable.Local
            IUseCaseTuner useCaseTuner, 
            IOperationScopeFactory scopeFactory)
        {
            _logger = logger;
            _finder = finder;
            _unitOfWork = unitOfWork;
            _orderValidationRepository = orderValidationRepository;
            _allValidationRules = allValidationRules;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
        }

        public ValidateOrdersResult ValidateOrders(OrderValidationPredicate filterPredicate, ValidateOrdersRequest request)
        {
            _useCaseTuner.AlterDuration<OrderValidationService>();
            
            var rules = _allValidationRules.Where(x => ValidationTypesMap[request.Type].Contains(x.GetType())).ToArray();

            int orderCount;
            ValidResultsContainer validResultsContainer;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                validResultsContainer = _orderValidationRepository.CreateValidResultsContainer(filterPredicate.GetCombinedPredicate(), out orderCount);
                transaction.Complete();
            }

            _logger.InfoFormatEx("Starting order validation. Type: {0}. Orders to validate: {1}", request.Type, orderCount);
            var stopwatch = Stopwatch.StartNew();

            var validationMessages = new List<OrderValidationMessage>();
            var ruleGroupMappings = GetRuleGroupMappings();
            
            foreach (var ruleGroupMapping in ruleGroupMappings)
            {
                var groupCode = ruleGroupMapping.Key;

                // список заказов с ошибками, для проверки кол-ва рекламы
                var invalidOrderIdsForAdvAmountValidation = OrderValidationRuleGroup.AdvertisementAmountValidation == groupCode
                                                                ? validationMessages.Where(x => x.Type == MessageType.Error)
                                                                                    .Select(x => x.OrderId)
                                                                                    .Distinct().ToArray()
                                                                : null;

                            var validationGroupId = _orderValidationRepository.GetGroupId(groupCode);
                var combinedPredicate = new OrderValidationPredicate(filterPredicate.GeneralPart,
                                                             filterPredicate.OrgUnitPart,
                                                             x => !x.OrderValidationResults
                                                                    .Where(y => y.OrderValidationGroupId == validationGroupId)
                                                                    .OrderByDescending(y => y.Id)
                                                                    .Select(y => y.IsValid)
                                                                    .FirstOrDefault());

                var ordersToValidateCount = _finder.Find(combinedPredicate.GetCombinedPredicate()).Count();
                if (ordersToValidateCount == 0)
                {
                    _logger.InfoFormatEx("Skipping validating orders by group [{0}]. No orders to validate.", groupCode);
                    continue;
                }

                var groupMessages = new List<OrderValidationMessage>();
                var groupRuleCodes = ruleGroupMapping.Value.ToArray();

                _logger.InfoFormatEx("Validating orders by group [{0}]. Rules in group: [{1}]. Orders to validate: [{2}]", groupCode, groupRuleCodes.Length, ordersToValidateCount);
                var groupStopwatch = Stopwatch.StartNew();

                foreach (var ruleCode in groupRuleCodes)
                {
                    var ruleType = RuleCodeMap[ruleCode];
                    var rule = rules.SingleOrDefault(x => x.GetType() == ruleType);
                    if (rule == null)
                    {
                        continue;
                    }

                    var lastTime = groupStopwatch.ElapsedMilliseconds;
                    try
                    {
                        request.InvalidOrderIds = invalidOrderIdsForAdvAmountValidation;

                        using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                        {
                            var ruleMessages = rule.Validate(request, combinedPredicate);

                            foreach (var message in ruleMessages)
                            {
                                message.RuleCode = ruleCode;
                            }

                            groupMessages.AddRange(ruleMessages);
                            transaction.Complete();
                        }

                        var timeTaken = groupStopwatch.ElapsedMilliseconds - lastTime;
                        _logger.InfoFormatEx("Rule '{0}' executed in {1:F2} sec. OrganizationUnitId = [{2}]",
                                             rule.GetType().Name,
                                             timeTaken / 1000D,
                                             request.OrganizationUnitId);
                    }
                    catch (Exception ex)
                    {
                        var timeTaken = groupStopwatch.ElapsedMilliseconds - lastTime;
                        _logger.InfoFormatEx("Rule '{0}' failed after {1:F2} sec. OrganizationUnitId = [{2}]",
                             rule.GetType().Name,
                             timeTaken / 1000D,
                             request.OrganizationUnitId);

                        _logger.ErrorFormatEx(ex, "При выполнении проверки [{0}] произошла ошибка", rule.GetType().Name);
                        throw;
                    }
                }

                validationMessages.AddRange(groupMessages);

                Action appendValidResultsAction = () =>
                        {
                    var invalidOrderIds = groupMessages.Where(x => x.Type == MessageType.Error || x.Type == MessageType.Warning)
                                                                           .Select(x => x.OrderId)
                                                                           .Distinct()
                                                                           .ToArray();
                    validResultsContainer.AppendValidResults(invalidOrderIds, new ValidationContext(groupCode, request.Type));
                        };

                switch (groupCode)
                {
                    case OrderValidationRuleGroup.Generic:
                    case OrderValidationRuleGroup.AdvertisementAmountValidation:
                        // Сохраняем результаты пройденных проверок только для конкретных групп, 
                        // для которых определены события сброса признака корректности группы
                        break;
                    case OrderValidationRuleGroup.ADPositionsValidation:
                        // Для группы СЗП сохраняем результаты пройденных проверок только в случае запуска перед сборкой, т.к.
                        // существуют ситуации для региональных заказов, когда заказ является валидным с точки зрения индивидуальной проверки,
                        // но при массовой проверке в выборку для валидации добавляются заказы, которые конфликтуют с этим заказом.
                        // Вероятно, ошибки здесь нет (т.к. для разных городов возможна продажа, например, запрещенных позиций),
                        // но для сохранения предсказуемого поведения на текущий момомент не сохраняем результаты индивидуальной проверки
                        if (request.Type == ValidationType.PreReleaseBeta ||
                            request.Type == ValidationType.PreReleaseFinal)
                        {
                            appendValidResultsAction();
                        }

                        break;
                    default:
                        // Для проверок, запущенных через UI признак корректности группы не выставляем, 
                        // т.к. в этом случае запускаются не все проверки в группе, и поэтому заказ не может считаться валидным с точки зрения этой группы
                        if (request.Type != ValidationType.ManualReport &&
                            request.Type != ValidationType.ManualReportWithAccountsCheck)
                        {
                            appendValidResultsAction();
                        }

                        break;
                }

                groupStopwatch.Stop();
                _logger.InfoFormatEx("Validating orders by group [{0}] completed in {1:F2} sec.", groupCode, groupStopwatch.ElapsedMilliseconds / 1000D);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
            using (var scope = _unitOfWork.CreateScope())
            {
                var scopedOrderValidationRepository = scope.CreateRepository<IOrderValidationRepository>();
                scopedOrderValidationRepository.SaveValidResults(validResultsContainer);
                scope.Complete();
            }

                transaction.Complete();
            }

            stopwatch.Stop();
            _logger.InfoFormatEx("Order validation completed in {0:F2} sec.", stopwatch.ElapsedMilliseconds / 1000D);

            // Вынесем ошибки не связанные с заказом в конец, сохраняя порядок
            var errorsList = validationMessages.Where(x => x.OrderId != 0).ToList();
            errorsList.AddRange(validationMessages.Where(x => x.OrderId == 0).ToArray());

            return new ValidateOrdersResult { OrderCount = orderCount, Messages = errorsList.ToArray() };
        }

        public void SetGroupAsInvalid(long orderId, OrderValidationRuleGroup orderValidationRuleGroup)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ResetValidationGroupIdentity>())
            {
                // Отметка группы как невалидной осуществляется только в случае одного заказа при выполнении бизнес-операции над этим заказом
                // Однако заказ может быть не только На оформлении, но и в других статусах
                _orderValidationRepository.AddInvalidResult(orderId, new ValidationContext(orderValidationRuleGroup, ValidationType.SingleOrderOnRegistration));

                scope.Complete();
            }
        }

        private Dictionary<OrderValidationRuleGroup, IEnumerable<int>> GetRuleGroupMappings()
        {
            return _finder.FindAll<DoubleGis.Erm.Platform.Model.Entities.Erm.OrderValidationRuleGroup>()
                .Select(x => new
                    {
                        GroupCode = x.Code,
                    RuleCodes = x.OrderValidationRuleGroupDetails.Select(y => y.RuleCode),
                    })
                .OrderBy(x => x.GroupCode)
                .ToDictionary(x => (OrderValidationRuleGroup)x.GroupCode, x => x.RuleCodes);
        }
    }
}
