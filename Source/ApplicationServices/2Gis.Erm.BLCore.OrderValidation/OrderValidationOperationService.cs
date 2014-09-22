using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class OrderValidationOperationService : IOrderValidationOperationService, IOrderValidationInvalidator
    {
        private readonly IOrderValidationRuleProvider _orderValidationRuleProvider;
        private readonly ICommonLog _logger;
        private readonly IFinder _finder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderValidationRepository _orderValidationRepository;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IOperationScopeFactory _scopeFactory;

        // FIXME {all, 03.03.2014}: ответсвенность по сбросу кэша проверок лучше вынести в отдельный сервис и т.п.
        // FIXME {all, 03.03.2014}: нужно избавиться от явно использования finder, и UoW (отложенное сохранение лучше запилить явно, через AggregateService явно)
        public OrderValidationOperationService(
            IOrderValidationRuleProvider orderValidationRuleProvider,
            ICommonLog logger,
            IFinder finder,
            IUnitOfWork unitOfWork,
            IOrderValidationRepository orderValidationRepository,
            IUseCaseTuner useCaseTuner, 
            IOperationScopeFactory scopeFactory)
        {
            _orderValidationRuleProvider = orderValidationRuleProvider;
            _logger = logger;
            _finder = finder;
            _unitOfWork = unitOfWork;
            _orderValidationRepository = orderValidationRepository;
            _useCaseTuner = useCaseTuner;
            _scopeFactory = scopeFactory;
        }

        public ValidateOrdersResult ValidateOrders(OrderValidationPredicate filterPredicate, ValidateOrdersRequest request)
        {
            _useCaseTuner.AlterDuration<OrderValidationOperationService>();
            
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

            var ruleGroupContainers = _orderValidationRuleProvider.GetAppropriateRules(validationType);
            foreach (var ruleGroupContainer in ruleGroupContainers)
            {
                if (!ruleGroupContainer.RuleDescriptors.Any())
                {
                    _logger.InfoFormatEx("Validation request token: {0}. Skip validation by group [{1}]. Appropriate rules count: 0.", request.Token, ruleGroupContainer.Group);
                    continue;
                }

                // список заказов с ошибками, для проверки кол-ва рекламы
                // FIXME {all, 12.02.2014}: invalidOrderIds по факту нужна одной проверке, но почему-то передается через весь стек вызовов здесь и, далее, всем проверкам
                //                          Раньше invalidOrderIds передавались неявно в составе ValidateOrdersRequest, теперь явно - чтобы обязательно вызывать вопросы
                // FIXME {all, 03.03.2014}: кроме того фактически есть завязка на детали работы GetRuleGroupMappings - нужно чтобы группа проверок, OrderValidationRuleGroup.AdvertisementAmountValidation всегда шла последней 
                // что достигается значением OrderValidationRuleGroup.AdvertisementAmountValidation, которое пока максимально среди всех значений кодов групп проверок, т.е. при добавлении новой группы и т.п. никаких гарантий правильного порядка выполнения проверок нет - полностью ручное управление - высокий риск fail
                // Кроме того, если есть фактически взаимозависомость по данным между проверками, то скорее всего требуется как-то явно организовать шаринг данных (в данном 
                // конкретном случае вызывает вопросы одновременное наличие списка заказов успешно прошедших проверки, и списка заказов с ошибками)
                var invalidOrderIds = OrderValidationRuleGroup.AdvertisementAmountValidation == ruleGroupContainer.Group
                                                                ? validationMessages.Where(x => x.Type == MessageType.Error)
                                                                                    .Select(x => x.OrderId)
                                                                                    .Distinct().ToArray()
                                                                : null;

                var groupMessages = ValidateByRuleGroup(validationType,
                                                        ruleGroupContainer,
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

        private IEnumerable<OrderValidationMessage> ValidateByRuleGroup(ValidationType validationType,
                                                                        OrderValidationRulesContianer ruleGroupContainer,
                                                                        OrderValidationPredicate filterPredicate,
                                                                        ValidateOrdersRequest request,
                                                                        IEnumerable<long> invalidOrderIds,
                                                                        ValidResultsContainer validResultsContainer)
        {
            bool useCachedResultsDisabled = !ruleGroupContainer.UseCaching || ruleGroupContainer.RuleDescriptors.Any(rd => !rd.UseCaching);

            // FIXME {all, 03.03.2014}: нужно использовать другой механизм сортировки кэша результатов проверок, т.к. сортировка по ID не дает гарантии, что будет учтен именно результат самой последней проверки 
            var combinedPredicate = 
                !useCachedResultsDisabled 
                    ? new OrderValidationPredicate(filterPredicate.GeneralPart,
                                                   filterPredicate.OrgUnitPart,
                                                   x => !x.OrderValidationResults
                                                                .Where(y => y.OrderValidationGroupId == (int)ruleGroupContainer.Group)
                                                                .OrderByDescending(y => y.Id)
                                                                .Select(y => y.IsValid)
                                                                .FirstOrDefault()) 
                    : filterPredicate;

            var ordersToValidateCount = _finder.Find(combinedPredicate.GetCombinedPredicate()).Count();
            if (ordersToValidateCount == 0)
            {
                _logger.InfoFormatEx("Validation request token: {0}. Skipping validating orders by group [{1}]. No orders to validate.", request.Token, ruleGroupContainer.Group);
                return Enumerable.Empty<OrderValidationMessage>();
            }

            _logger.InfoFormatEx("Validation request token: {0}. Validating orders by group [{1}]. Rules in group count: [{2}]. Orders to validate: [{3}]",
                                 request.Token,
                                 ruleGroupContainer.Group,
                                 ruleGroupContainer.RuleDescriptors.Count,
                                 ordersToValidateCount);

            var groupStopwatch = Stopwatch.StartNew();

            var groupMessages = new List<OrderValidationMessage>();
            foreach (var ruleDescriptor in ruleGroupContainer.RuleDescriptors)
            {
                var ruleMessages = ValidateByRule(ruleDescriptor.Rule, combinedPredicate, request, invalidOrderIds, () => groupStopwatch.ElapsedMilliseconds);
                AttachRuleInfo2Messages(ruleDescriptor, ruleMessages);
                groupMessages.AddRange(ruleMessages);
            }

            if (ruleGroupContainer.UseCaching && ruleGroupContainer.AllRulesScheduled)
            {
                AppendValidationResults(validationType, ruleGroupContainer.Group, groupMessages, validResultsContainer);
            }

            groupStopwatch.Stop();
            _logger.InfoFormatEx(
                "Validation request token: {0}. Validating orders by group [{1}] completed in {2:F2} sec.", 
                request.Token,
                ruleGroupContainer.Group, 
                groupStopwatch.ElapsedMilliseconds / 1000D);

            return groupMessages;
        }

        private void AttachRuleInfo2Messages(OrderValidationRuleDescritpor ruleDescriptor, IEnumerable<OrderValidationMessage> validationMessages)
        {
            foreach (var message in validationMessages)
            {
                message.RuleCode = ruleDescriptor.RuleCode;
            }
        }

        private IReadOnlyList<OrderValidationMessage> ValidateByRule(IOrderValidationRule rule,
                                                                   OrderValidationPredicate combinedPredicate,
                                                                   ValidateOrdersRequest request,
                                                                   IEnumerable<long> invalidOrderIds,
                                                                   Func<long> getElapsedMillisecondsFunc)
        {
            IReadOnlyList<OrderValidationMessage> ruleMessages;
            var lastTime = getElapsedMillisecondsFunc();
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
                {
                    ruleMessages = rule.Validate(combinedPredicate, invalidOrderIds, request);
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
