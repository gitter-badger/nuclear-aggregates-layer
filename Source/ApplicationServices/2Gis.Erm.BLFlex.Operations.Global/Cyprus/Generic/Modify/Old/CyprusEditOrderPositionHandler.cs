using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.Old
{
    // FIXME {all, 13.11.2013}: приехало из 1.0 - больше похоже на почти полную копипасту (нет сделок и т.п.) - необходимо проверить на соблюдение целостности BusinessModel
    public sealed class CyprusEditOrderPositionHandler : RequestHandler<EditOrderPositionRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly IPublicService _publicService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderValidationInvalidator _orderValidationInvalidator;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ISupportedCategoriesChecker _supportedCategoriesChecker;
        private readonly ICalculateCategoryRateService _calculateCategoryRateService;
        private readonly IFirmReadModel _firmReadModel;

        public CyprusEditOrderPositionHandler(IFinder finder,
                                              IPublicService publicService,
                                              IOrderRepository orderRepository,
                                              IOrderReadModel orderReadModel,
                                              IOrderValidationInvalidator orderValidationInvalidator,
                                              IOperationScopeFactory scopeFactory,
                                              IPriceReadModel priceReadModel,
                                              ISupportedCategoriesChecker supportedCategoriesChecker,
                                              ICalculateCategoryRateService calculateCategoryRateService,
                                              IFirmReadModel firmReadModel)
        {
            _finder = finder;
            _publicService = publicService;
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
            _orderValidationInvalidator = orderValidationInvalidator;
            _scopeFactory = scopeFactory;
            _priceReadModel = priceReadModel;
            _supportedCategoriesChecker = supportedCategoriesChecker;
            _calculateCategoryRateService = calculateCategoryRateService;
            _firmReadModel = firmReadModel;
        }

        protected override EmptyResponse Handle(EditOrderPositionRequest request)
        {
            var orderPosition = request.Entity;
            var advertisementsLinks = request.AdvertisementsLinks;

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(orderPosition.OrderId))
                                   .Select(x => new
                                   {
                                       x.Id,
                                       x.FirmId,
                                       x.WorkflowStepId,
                                       x.ReleaseCountFact,
                                       x.DealId,
                                       x.EndDistributionDateFact,
                                       x.OwnerCode,
                                       x.DestOrganizationUnitId,
                                       x.PlatformId,
                                       x.BudgetType,
                                       OrderType = (OrderType)x.OrderType
                                   })
                                   .Single();

            var subRequest = new CanCreateOrderPositionForOrderRequest
            {
                OrderId = orderPosition.OrderId,
                OrderType = orderInfo.OrderType,
                FirmId = orderInfo.FirmId,
                OrderPositionCategoryIds = advertisementsLinks
                    .Where(x => x.CategoryId.HasValue)
                    .Select(x => x.CategoryId.Value)
                    .ToArray(),
                OrderPositionFirmAddressIds = advertisementsLinks
                    .Where(x => x.FirmAddressId.HasValue)
                    .Select(x => x.FirmAddressId.Value)
                    .ToArray(),

                IsPositionComposite = _finder.Find<PricePosition>(x => x.Id == orderPosition.PricePositionId).Select(x => x.Position.IsComposite).Single(),
                AdvertisementLinksCount = advertisementsLinks.Count()
            };

            var canCreateResponse = (CanCreateOrderPositionForOrderResponse)_publicService.Handle(subRequest);
            if (!canCreateResponse.CanCreate)
            {
                throw new NotificationException(string.Format(BLResources.CannotCreateOrderPositionTemplate, canCreateResponse.Message));
            }

            if (request.CategoryId != null)
            {
                _supportedCategoriesChecker.Check(_priceReadModel.GetPricePositionRateType(orderPosition.PricePositionId), request.CategoryId.Value, orderInfo.DestOrganizationUnitId);
            }

            if (orderInfo.WorkflowStepId != (int)OrderState.OnRegistration)
            {
                // Во избежание несанкционированных изменений в позиции заказа, прошедшего этап "на оформлении",
                // откатываем состояние сущности к тому, что лежит вместо того, что пришло от клиента
                orderPosition = _finder.Find(Specs.Find.ById<OrderPosition>(orderPosition.Id)).Single();
            }

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(orderPosition))
            {
                // Сохранение рекламы должно быть до расчета списаний
                var isOrderPositionCreated = !orderPosition.IsNew();
                if (isOrderPositionCreated)
                {
                    SetAdsValidationRuleGroupAsInvalid(orderInfo.Id);

                    var orderIsLocked = orderInfo.WorkflowStepId != (int)OrderState.OnRegistration;
                    _orderRepository.CreateOrUpdateOrderPositionAdvertisements(orderPosition.Id, advertisementsLinks, orderIsLocked);
                }

                if (orderInfo.WorkflowStepId == (int)OrderState.OnRegistration)
                {
                    orderPosition.OwnerCode = orderInfo.OwnerCode;

                    CheckAgainstOtherOrderPositions(orderPosition, orderInfo.Id, orderInfo.BudgetType, orderInfo.PlatformId);

                    var pricePositionInfo = _finder.Find(Specs.Find.ById<PricePosition>(orderPosition.PricePositionId))
                                                   .Select(
                                                       x =>
                                                       new
                                                       {
                                                           x.Cost,
                                                           x.Position.AccountingMethodEnum,
                                                           x.Price.OrganizationUnitId,
                                                           x.Position.IsComposite,
                                                           x.PositionId,
                                                           x.PriceId
                                                       })
                                                   .Single();

                    if (orderInfo.DestOrganizationUnitId != pricePositionInfo.OrganizationUnitId)
                    {
                        throw new NotificationException(BLResources.OrderOrganizationUnitDiffersFromPricesOne);
                    }

                    var categoryRate = _calculateCategoryRateService.CalculateCategoryRate(_firmReadModel.GetOrderFirmId(request.Entity.OrderId),
                                               request.Entity.PricePositionId,
                                               request.CategoryId,
                                               true);

                    var calculateOrderPositionPricesResponse =
                        (CalculateOrderPositionPricesResponse)_publicService.Handle(new CalculateOrderPositionPricesRequest
                        {
                            Cost = pricePositionInfo.Cost,
                            DiscountPercent = orderPosition.DiscountPercent,
                            DiscountSum = orderPosition.DiscountSum,
                            CalculateDiscountViaPercent = orderPosition.CalculateDiscountViaPercent,
                            OrderId = orderPosition.OrderId,
                            CategoryRate = categoryRate,
                            Amount = orderPosition.Amount
                        });

                    orderPosition.CategoryRate = calculateOrderPositionPricesResponse.CategoryRate;
                    orderPosition.ShipmentPlan = calculateOrderPositionPricesResponse.ShipmentPlan;
                    orderPosition.PricePerUnit = calculateOrderPositionPricesResponse.PricePerUnit;
                    orderPosition.PayablePrice = calculateOrderPositionPricesResponse.PayablePrice;
                    orderPosition.PricePerUnitWithVat = calculateOrderPositionPricesResponse.PricePerUnitWithVat;
                    orderPosition.PayablePlan = calculateOrderPositionPricesResponse.PayablePlan;
                    orderPosition.PayablePlanWoVat = calculateOrderPositionPricesResponse.PayablePlanWoVat;
                    orderPosition.DiscountPercent = calculateOrderPositionPricesResponse.DiscountPercent;
                    orderPosition.DiscountSum = calculateOrderPositionPricesResponse.DiscountSum;


                    ValidateEntity(orderPosition, pricePositionInfo.AccountingMethodEnum == (int)PositionAccountingMethod.PlannedProvision);

                    // Сохраняем изменения OrderPosition в БД
                    _orderRepository.CreateOrUpdate(orderPosition);

                    if (!isOrderPositionCreated)
                    {
                        SetAdsValidationRuleGroupAsInvalid(orderInfo.Id);

                        var orderIsLocked = orderInfo.WorkflowStepId != (int)OrderState.OnRegistration;
                        _orderRepository.CreateOrUpdateOrderPositionAdvertisements(orderPosition.Id, advertisementsLinks, orderIsLocked);
                    }

                    var order = _orderReadModel.GetOrder(orderPosition.OrderId);

                    // TODO : разобраться - проверка в методе CheckAgainstOtherOrderPositions основывается на
                    // старых значениях PlatformId и BudgetType, а теперь эти поля могут измениться - как бы чего не вышло.
                    _orderReadModel.DetermineOrderBudgetType(order);
                    _orderReadModel.DetermineOrderPlatform(order);

                    _orderRepository.UpdateOrderNumber(order);
                    _publicService.Handle(new UpdateOrderFinancialPerformanceRequest { Order = order, ReleaseCountFact = orderInfo.ReleaseCountFact });
                    _publicService.Handle(new CalculateReleaseWithdrawalsRequest { Order = order });

                    if (orderInfo.DealId != null)
                    {
                        // FIXME {all, 13.11.2013}: приехало из 1.0 - на кипре нет сделок, непонятно зачем вызывается в таком случае данный функционал
                        // было _publicService.Handle(new UpdateDealsOnWithdrawalRequest { DealIds = new[] { orderInfo.DealId.Value } });
                        _publicService.Handle(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { orderInfo.DealId.Value } });
                    }

                    // Сохраняем изменения объектов Order  в БД, если по каким-то причинам это не сделал один из вышестоящих хендлеров
                    _orderRepository.Update(order);

                    // TODO: Сделать поток выполнения прозрачным!
                }

                if (isOrderPositionCreated)
                {
                    operationScope
                        .Updated<OrderPosition>(orderPosition.Id);
                }
                else
                {
                    operationScope
                        .Added<OrderPosition>(orderPosition.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }

        private static void ValidateEntity(OrderPosition entity, bool isBudget)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.Amount <= 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveExistingOpderPositionsWithZeroCount);
            }

            if (entity.DiscountSum < 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveOrderPositionWithNegativeDiscount);
            }

            if (entity.DiscountPercent < 0 || entity.DiscountPercent > 100)
            {
                throw new NotificationException(BLResources.AttemptToSaveOrderPositionsWithInvalidDiscountRate);
            }

            if (entity.PayablePlan < 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveOrderPositionWithNegativeValueOfPayablePrice);
            }

            if (!isBudget)
            {
                return;
            }

            if (entity.Amount != 1)
            {
                throw new NotificationException(BLResources.AttemptToSaveBudgeteOrderPositionWithCountNotEqualToOne);
            }

            if (entity.DiscountSum != 0 || entity.DiscountPercent != 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveBudgeteOrderPositionWithNonZeroValueOfDiscount);
            }

            if (entity.PricePerUnitWithVat < 0 || entity.PricePerUnitWithVat < 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveBudgeteOrderPositionWithNegativePrice);
            }
        }

        private void CheckAgainstOtherOrderPositions(OrderPosition entity, long orderId, int? budgetType, long? platformId)
        {
            var hasOtherPositions = _finder.Find(Specs.Find.ById<Order>(orderId))
                .SelectMany(order => order.OrderPositions)
                .Any(op => op.Id != entity.Id && !op.IsDeleted && op.IsActive);

            if (budgetType != (int)OrderBudgetType.None)
            {
                if (hasOtherPositions)
                {
                    var accountingMethod = _finder.Find(Specs.Find.ById<PricePosition>(entity.PricePositionId))
                                                .Select(pp => (PositionAccountingMethod)pp.Position.AccountingMethodEnum)
                                                .FirstOrDefault();

                    if (accountingMethod == PositionAccountingMethod.PlannedProvision && budgetType != (int)OrderBudgetType.Budget)
                    {
                        throw new NotificationException(BLResources.OrderSellCanAcceptOnlySellPositions);
                    }

                    if (accountingMethod == PositionAccountingMethod.GuaranteedProvision && budgetType != (int)OrderBudgetType.Sell)
                    {
                        throw new NotificationException(BLResources.OrderBudgetCanAcceptOnlyBudgetPositions);
                    }
                }
            }
        }

        private void SetAdsValidationRuleGroupAsInvalid(long orderId)
        {
            _orderValidationInvalidator.Invalidate(new []{ orderId }, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
        }
    }
}