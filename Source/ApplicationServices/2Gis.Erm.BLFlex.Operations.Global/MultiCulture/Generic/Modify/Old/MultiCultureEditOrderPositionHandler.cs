using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.Old
{
    // FIXME {all, 10.07.2014}: почти полная copy/paste других adapted версий этого handler, при рефакторинге ApplicationServices - попытаться объеденить обратно + использование finder и т.п.

    public sealed class MultiCultureEditOrderPositionHandler : RequestHandler<EditOrderPositionRequest, EmptyResponse>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        private readonly IPublicService _publicService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;

        private readonly IOrderValidationInvalidator _orderValidationInvalidator;
        private readonly IOperationScopeFactory _scopeFactory;

        public MultiCultureEditOrderPositionHandler(IFinder finder,
                                                    IOrderReadModel orderReadModel,
                                                    IPositionReadModel positionReadModel,
                                                    IOrganizationUnitReadModel organizationUnitReadModel,
                                                    IPublicService publicService,
                                                    IOrderRepository orderRepository,
                                                    ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
                                                    IOrderValidationInvalidator orderValidationInvalidator,
                                                    IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _orderReadModel = orderReadModel;
            _positionReadModel = positionReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _orderValidationInvalidator = orderValidationInvalidator;
            _publicService = publicService;
            _orderRepository = orderRepository;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _scopeFactory = scopeFactory;
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
                                       x.EndDistributionDateFact,
                                       x.OwnerCode,
                                       x.DestOrganizationUnitId,
                                       x.PlatformId,
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

                    var pricePositionInfo = _finder.Find(Specs.Find.ById<PricePosition>(orderPosition.PricePositionId))
                                                   .Select(
                                                       x =>
                                                       new
                                                       {
                                                           x.Cost,
                                                           x.Position.AccountingMethodEnum,
                                                           x.Price.OrganizationUnitId,
                                                           x.RateType,
                                                           x.Position.IsComposite,
                                                           x.PositionId,
                                                           x.PriceId
                                                       })
                                                   .Single();

                    if (orderInfo.DestOrganizationUnitId != pricePositionInfo.OrganizationUnitId)
                    {
                        throw new NotificationException(BLResources.OrderOrganizationUnitDiffersFromPricesOne);
                    }

                    if (request.CategoryIds.Any())
                    {
                        var unsupported = _positionReadModel.GetNewSalesModelDeniedCategories((PositionAccountingMethod)pricePositionInfo.AccountingMethodEnum,
                                                                                      orderInfo.DestOrganizationUnitId,
                                                                                      request.CategoryIds);
                        if (unsupported.Any())
                        {
                            var organizationUnitName = _organizationUnitReadModel.GetName(orderInfo.DestOrganizationUnitId);
                            throw new NewSalesModelNotEnabledForCategoryOrOrganizationUnitException(unsupported.Select(pair => pair.Value), organizationUnitName);
                        }
                    }

                    var categoryRate = _calculateCategoryRateOperationService.GetCategoryRateForOrderCalculated(request.Entity.OrderId,
                                               request.Entity.PricePositionId,
                                               request.CategoryIds);

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

                    _orderReadModel.UpdateOrderPlatform(order);

                    _orderRepository.UpdateOrderNumber(order);
                    _publicService.Handle(new UpdateOrderFinancialPerformanceRequest { Order = order, ReleaseCountFact = orderInfo.ReleaseCountFact });
                    _publicService.Handle(new CalculateReleaseWithdrawalsRequest { Order = order });

                    // Сохраняем изменения объектов Order  в БД, если по каким-то причинам это не сделал один из вышестоящих хендлеров
                    _orderRepository.Update(order);
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

            if (entity.PricePerUnitWithVat < 0 || entity.PricePerUnitWithVat < 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveBudgeteOrderPositionWithNegativePrice);
            }
        }

        private void SetAdsValidationRuleGroupAsInvalid(long orderId)
        {
            _orderValidationInvalidator.Invalidate(new[] { orderId }, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
        }
    }
}