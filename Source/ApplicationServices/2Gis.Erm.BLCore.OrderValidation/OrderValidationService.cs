using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
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
    public sealed class OrderValidationService : IOrderValidationService, IOrderValidationInvalidator
    {
        // FIXME {all, 03.03.2014}: фактически сервис проверок занимается и хостингом маппингов + достает детали настройки групп проверок из persistence, сопоставляет их с типом запущенной проверки и т.п. 
        // итого видится более правильным вынести механизм получения списка rule instances для выполнения проверок в некий провайдер.
        // провайдер уже может получать данные о правилах фактически от massprocessor rules + используя readmodel вычитывать из persistence, (а возможно из config файла настройки активных групп)
        // сервису проверок останется всего лишь запрашивать нужный набор правил, указываея детали нужного режима проверок
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
                { 21, typeof(AdvertisementsOnlyWhiteListOrderValidationRule) },
                { 22, typeof(AdvertisementsWithoutWhiteListOrderValidationRule) },
                { 23, typeof(LegalPersonProfilesOrderValidationRule) },
                { 24, typeof(WarrantyEndDateOrderValidationRule) },
                { 25, typeof(BargainEndDateOrderValidationRule) },
                { 26, typeof(AdvertisementAmountOrderValidationRule) },
                { 27, typeof(ContactDoesntContainSponsoredLinkOrderValidationRule) },
                { 29, typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule) },
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
            };

        private static readonly Type[] CommonRules =
            {
                typeof(AssociatedAndDeniedPricePositionsOrderValidationRule),
                typeof(FirmBelongsToOrdersDestOrgUnitOrderValidationRule),
                typeof(AdvertisementsOnlyWhiteListOrderValidationRule),
                typeof(AdvertisementsWithoutWhiteListOrderValidationRule),
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
                typeof(CouponIsUniqueForFirmOrderValidationRule)
            };

        private static readonly Type[] SingleOrderValidationRules =
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
                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule)
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
                        .Concat(SingleOrderValidationRules)
                        .ToArray()
                },
                {
                    ValidationType.SingleOrderOnStateChanging,

                    new[] { typeof(AssociatedAndDeniedPricePositionsOrderValidationRule) }
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
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule)
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
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule)
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
                                typeof(AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule)
                            })
                        .ToArray()
                }
            };

        #endregion

        private readonly ICommonLog _logger;
        private readonly IFinder _finder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderValidationRepository _orderValidationRepository;
        private readonly IEnumerable<IOrderValidationRule> _allValidationRules;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;

        // FIXME {all, 03.03.2014}: ответсвенность по сбросу кэша проверок лучше вынести в отдельный сервис и т.п.
        // FIXME {all, 03.03.2014}: нужно избавиться от явно использования finder, и UoW (отложенное сохранение лучше запилить явно, через AggregateService явно)
        public OrderValidationService(ICommonLog logger,
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
            
            var validationType = request.Type;

            // TODO {all, 03.03.2014}: большой вопрос корректно ли считывайть кэш, выполнять проверки, актуализировать кэш в разных транзакциях с точки зрения именно бизнесс операции, т.е. слоя OperationServices
            int orderCount;
            var validResultsContainer = CreateValidResultsContainer(filterPredicate, out orderCount);

            _logger.InfoFormatEx("Starting order validation. Type: {0}. Orders to validate: {1}. Request token: {2}", validationType, orderCount, request.Token);
            _logger.InfoEx("Validation details: " + request.AsDescription());

            var stopwatch = Stopwatch.StartNew();
            var validationMessages = ValidateOrders(validationType, filterPredicate, request, validResultsContainer);
            stopwatch.Stop();

            _logger.InfoFormatEx("Order validation request {0} completed in {1:F2} sec.", request.Token, stopwatch.ElapsedMilliseconds / 1000D);
            
            stopwatch.Start();
            CacheValidResults(validResultsContainer);
            stopwatch.Stop();

            _logger.InfoFormatEx("Order validation request {0} results cached in {1:F2} sec.", request.Token, stopwatch.ElapsedMilliseconds / 1000D);
            
            return ConstructValidateOrdersResult(validationMessages, orderCount);
        }

        public void Invalidate(IEnumerable<long> orderIds, OrderValidationRuleGroup orderValidationRuleGroup)
        {
            // FIXME {d.ivanov, 03.03.2014}: Та ли OperationIdentity здесь использована, вомзожно нужно ResetOrderValidationResultsIdentity 
            // если Identity использована не та, то скорее всего ResetValidationGroupIdentity уже утратила актуальность - и эта операция obsolete (а, если их нет на production то её вообще следует удалить)
            // DONE {i.maslennikov, 04.03.2014}: OperationIdentity использована та, только названа она криво. Переименовал из ResetValidationGroupIdentity в SetRuleGroupAsInvalidIdentity, оставил тот же Id
            // Удалить конечно нельзя, было уже выполнено множество сборок
            using (var scope = _scopeFactory.CreateNonCoupled<SetRuleGroupAsInvalidIdentity>())
            {
                // Отметка группы как невалидной осуществляется только в случае одного заказа при выполнении бизнес-операции над этим заказом
                // Однако заказ может быть не только На оформлении, но и в других статусах
                var validationContext = new ValidationContext(orderValidationRuleGroup, ValidationType.SingleOrderOnRegistration);
                foreach (var orderId in orderIds)
                {
                    _orderValidationRepository.AddInvalidResult(orderId, validationContext);
                }

                scope.Complete();
            }
        }

        private static void AppendValidationResults(ValidationType validationType,
                                                    OrderValidationRuleGroup ruleGroup,
                                                    IEnumerable<OrderValidationMessage> groupMessages,
                                                    ValidResultsContainer validResultsContainer)
        {
            var invalidOrderIds = groupMessages.Where(x => x.Type == MessageType.Error || x.Type == MessageType.Warning)
                                               .Select(x => x.OrderId)
                                               .Distinct()
                                               .ToArray();
            validResultsContainer.AppendValidResults(invalidOrderIds, new ValidationContext(ruleGroup, validationType));
        }

        private static ValidateOrdersResult ConstructValidateOrdersResult(List<OrderValidationMessage> validationMessages, int orderCount)
        {
            // Вынесем ошибки не связанные с заказом в конец, сохраняя порядок
            var errorsList = validationMessages.Where(x => x.OrderId != 0).ToList();
            errorsList.AddRange(validationMessages.Where(x => x.OrderId == 0).ToArray());

            return new ValidateOrdersResult { OrderCount = orderCount, Messages = errorsList.ToArray() };
        }

        private List<OrderValidationMessage> ValidateOrders(ValidationType validationType,
                                                            OrderValidationPredicate filterPredicate,
                                                            ValidateOrdersRequest request,
                                                            ValidResultsContainer validResultsContainer)
        {
            var validationMessages = new List<OrderValidationMessage>();

            var rules = _allValidationRules.Where(x => ValidationTypesMap[validationType].Contains(x.GetType())).ToDictionary(x => x.GetType(), x => x);
            var ruleGroupMappings = GetRuleGroupMappings();
            foreach (var ruleGroupMapping in ruleGroupMappings)
            {
                var groupCode = ruleGroupMapping.Key;
                var ruleCodes = ruleGroupMapping.Value;
                var groupRules = ruleCodes.Aggregate(new List<IOrderValidationRule>(),
                                                     (list, nextRuleCode) =>
                                                         {
                                                             IOrderValidationRule rule;
                                                             if (rules.TryGetValue(RuleCodeMap[nextRuleCode], out rule))
                                                             {
                                                                 list.Add(rule);
                                                             }

                                                             return list;
                                                         })
                                            .ToArray();

                if (!groupRules.Any())
                {
                    _logger.InfoFormatEx("Validation request token: {0}. Skip validation by group [{1}]. Appropriate rules count: 0.", request.Token, groupCode);
                    continue;
                }

                // список заказов с ошибками, для проверки кол-ва рекламы
                // FIXME {all, 12.02.2014}: invalidOrderIds по факту нужна одной проверке, но почему-то передается через весь стек вызовов здесь и, далее, всем проверкам
                //                          Раньше invalidOrderIds передавались неявно в составе ValidateOrdersRequest, теперь явно - чтобы обязательно вызывать вопросы
                // FIXME {all, 03.03.2014}: кроме того фактически есть завязка на детали работы GetRuleGroupMappings - нужно чтобы группа проверок, OrderValidationRuleGroup.AdvertisementAmountValidation всегда шла последней 
                // что достигается значением OrderValidationRuleGroup.AdvertisementAmountValidation, которое пока максимально среди всех значений кодов групп проверок, т.е. при добавлении новой группы и т.п. никаких гарантий правильного порядка выполнения проверок нет - полностью ручное управление - высокий риск fail
                // Кроме того, если есть фактически взаимозависомость по данным между проверками, то скорее всего требуется как-то явно организовать шаринг данных (в данном 
                // конкретном случае вызывает вопросы одновременное наличие списка заказов успешно прошедших проверки, и списка заказов с ошибками)
                var invalidOrderIds = OrderValidationRuleGroup.AdvertisementAmountValidation == groupCode
                                                                ? validationMessages.Where(x => x.Type == MessageType.Error)
                                                                                    .Select(x => x.OrderId)
                                                                                    .Distinct().ToArray()
                                                                : null;

                var groupMessages = ValidateByRuleGroup(groupCode,
                                                        groupRules,
                                                        validationType,
                                                        filterPredicate,
                                                        request,
                                                        invalidOrderIds,
                                                        validResultsContainer);
                validationMessages.AddRange(groupMessages);
            }
            
            return validationMessages;
        }

        private ValidResultsContainer CreateValidResultsContainer(OrderValidationPredicate filterPredicate, out int orderCount)
        {
            ValidResultsContainer validResultsContainer;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                validResultsContainer = _orderValidationRepository.CreateValidResultsContainer(filterPredicate.GetCombinedPredicate(), out orderCount);
                transaction.Complete();
            }

            return validResultsContainer;
        }

        private Dictionary<OrderValidationRuleGroup, IEnumerable<int>> GetRuleGroupMappings()
        {
            return _finder.FindAll<Platform.Model.Entities.Erm.OrderValidationRuleGroup>()
                          .Select(x => new
                              {
                                  GroupCode = x.Code,
                                  RuleCodes = x.OrderValidationRuleGroupDetails.Select(y => y.RuleCode),
                              })
                          .OrderBy(x => x.GroupCode)
                          .ToDictionary(x => (OrderValidationRuleGroup)x.GroupCode, x => x.RuleCodes);
        }

        private IEnumerable<OrderValidationMessage> ValidateByRuleGroup(OrderValidationRuleGroup groupCode,
                                                                        IOrderValidationRule[] rules,
                                                                        ValidationType validationType,
                                                                        OrderValidationPredicate filterPredicate,
                                                                        ValidateOrdersRequest request,
                                                                        IEnumerable<long> invalidOrderIds,
                                                                        ValidResultsContainer validResultsContainer)
        {
            var validationGroupId = _orderValidationRepository.GetGroupId(groupCode);

            // FIXME {all, 03.03.2014}: нужно использовать другой механизм сортировки кэша результатов проверок, т.к. сортировка по ID не дает гарантии, что будет учтен именно результат самой последней проверки 
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
                _logger.InfoFormatEx("Validation request token: {0}. Skipping validating orders by group [{1}]. No orders to validate.", request.Token, groupCode);
                return Enumerable.Empty<OrderValidationMessage>();
            }

            _logger.InfoFormatEx("Validation request token: {0}. Validating orders by group [{1}]. Rules in group count: [{2}]. Orders to validate: [{3}]",
                                 request.Token,
                                 groupCode,
                                 rules.Length,
                                 ordersToValidateCount);

            var groupStopwatch = Stopwatch.StartNew();

            var groupMessages = new List<OrderValidationMessage>();
            foreach (var rule in rules)
            {
                var ruleMessages = ValidateByRule(rule, combinedPredicate, request, invalidOrderIds, () => groupStopwatch.ElapsedMilliseconds);
                groupMessages.AddRange(ruleMessages);
            }

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
                    if (validationType == ValidationType.PreReleaseBeta || validationType == ValidationType.PreReleaseFinal)
                    {
                        AppendValidationResults(validationType, groupCode, groupMessages, validResultsContainer);
                    }

                    break;
                default:
                    // Для проверок, запущенных через UI признак корректности группы не выставляем, 
                    // т.к. в этом случае запускаются не все проверки в группе, и поэтому заказ не может считаться валидным с точки зрения этой группы
                    if (validationType != ValidationType.ManualReport && validationType != ValidationType.ManualReportWithAccountsCheck)
                    {
                        AppendValidationResults(validationType, groupCode, groupMessages, validResultsContainer);
                    }

                    break;
            }

            groupStopwatch.Stop();
            _logger.InfoFormatEx(
                "Validation request token: {0}. Validating orders by group [{1}] completed in {2:F2} sec.", 
                request.Token, 
                groupCode, 
                groupStopwatch.ElapsedMilliseconds / 1000D);

            return groupMessages;
        }

        private IEnumerable<OrderValidationMessage> ValidateByRule(IOrderValidationRule rule,
                                                                   OrderValidationPredicate combinedPredicate,
                                                                   ValidateOrdersRequest request,
                                                                   IEnumerable<long> invalidOrderIds,
                                                                   Func<long> getElapsedMillisecondsFunc)
        {
            IEnumerable<OrderValidationMessage> ruleMessages;
            var lastTime = getElapsedMillisecondsFunc();
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    ruleMessages = rule.Validate(combinedPredicate, invalidOrderIds, request);

                    foreach (var message in ruleMessages)
                    {
                        message.RuleCode = RuleCodeMap.Single(x => x.Value == rule.GetType()).Key;
                    }

                    transaction.Complete();
                }

                var timeTaken = getElapsedMillisecondsFunc() - lastTime;
                _logger.InfoFormatEx("Validation request token: {0}. Rule '{1}' executed in {2:F2} sec. OrganizationUnitId = [{3}]",
                                     request.Token,
                                     rule.GetType().Name,
                                     timeTaken / 1000D,
                                     request.OrganizationUnitId);
            }
            catch (Exception ex)
            {
                var timeTaken = getElapsedMillisecondsFunc() - lastTime;
                _logger.InfoFormatEx("Validation request token: {0}. Rule '{1}' failed after {2:F2} sec. OrganizationUnitId = [{3}]",
                                     request.Token,
                                     rule.GetType().Name,
                                     timeTaken / 1000D,
                                     request.OrganizationUnitId);

                _logger.ErrorFormatEx(ex, "При выполнении проверки [{0}] произошла ошибка. Validation request token: {1}", rule.GetType().Name, request.Token);
                throw;
            }

            return ruleMessages;
        }

        private void CacheValidResults(ValidResultsContainer validResultsContainer)
        {
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
        }
    }
}
