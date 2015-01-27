﻿using System;
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
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.Old
{
    // FIXME {all, 10.07.2014}: почти полная copy/paste других adapted версий этого handler, при рефакторинге ApplicationServices - попытаться объеденить обратно + использование finder и т.п.
    public sealed class EditOrderPositionHandler : RequestHandler<EditOrderPositionRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        private readonly IPublicService _publicService;
        private readonly IOrderRepository _orderRepository;
        private readonly IReplaceOrderPositionAdvertisementLinksOperationService _replaceOrderPositionAdvertisementLinksOperationService;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;
        private readonly ICalculateOrderPositionCostService _calculateOrderPositionCostService;
        private readonly IOperationScopeFactory _scopeFactory;

        public EditOrderPositionHandler(
            IFinder finder,
            IOrderReadModel orderReadModel,
            IPositionReadModel positionReadModel,
            IOrganizationUnitReadModel organizationUnitReadModel,
            IPublicService publicService,
            IOrderRepository orderRepository,
            IReplaceOrderPositionAdvertisementLinksOperationService replaceOrderPositionAdvertisementLinksOperationService,
            ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
            ICalculateOrderPositionCostService calculateOrderPositionCostService,
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _orderReadModel = orderReadModel;
            _positionReadModel = positionReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _publicService = publicService;
            _orderRepository = orderRepository;
            _replaceOrderPositionAdvertisementLinksOperationService = replaceOrderPositionAdvertisementLinksOperationService;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _calculateOrderPositionCostService = calculateOrderPositionCostService;
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
                    x.SourceOrganizationUnitId,
                    x.DestOrganizationUnitId,
                    x.PlatformId,
                    OrderType = x.OrderType
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

            if (orderInfo.WorkflowStepId != OrderState.OnRegistration)
            {
                // Во избежание несанкционированных изменений в позиции заказа, прошедшего этап "на оформлении",
                // откатываем состояние сущности к тому, что лежит вместо того, что пришло от клиента
                orderPosition = _finder.Find(Specs.Find.ById<OrderPosition>(orderPosition.Id)).Single();
            }

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(orderPosition))
            {
                // Сохранение рекламы должно быть до расчета списаний
                var isOrderPositionAlreadyExisting = !orderPosition.IsNew();
                if (isOrderPositionAlreadyExisting)
                {
                    _replaceOrderPositionAdvertisementLinksOperationService.Replace(orderPosition.Id, advertisementsLinks);
                }

                if (orderInfo.WorkflowStepId == OrderState.OnRegistration)
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
                        var unsupported = _positionReadModel.GetNewSalesModelDeniedCategories(pricePositionInfo.AccountingMethodEnum,
                                                                                      orderInfo.DestOrganizationUnitId,
                                                                                      request.CategoryIds);
                        if (unsupported.Any())
                        {
                            var organizationUnitName = _organizationUnitReadModel.GetName(orderInfo.DestOrganizationUnitId);
                            throw new NewSalesModelNotEnabledForCategoryOrOrganizationUnitException(unsupported.Select(pair => pair.Value), organizationUnitName);
                        }
                    }

                    // Делаем расчеты денег для позиции заказа
                    // Для пакета делаем разложение по номенклатурным позициям, делаем расчет для каждой подпозиции и затем суммируем то, что получилось
                    if (pricePositionInfo.IsComposite)
                    {
                        var positionInfo = new CalcPositionWithDiscountInfo
                        {
                            PositionInfo = new CalcPositionInfo
                            {
                                Amount = orderPosition.Amount,
                                PositionId = pricePositionInfo.PositionId,
                            },
                            DiscountInfo = new DiscountInfo
                            {
                                Percent = orderPosition.DiscountPercent,
                                Sum = orderPosition.DiscountSum,
                                CalculateDiscountViaPercent = orderPosition.CalculateDiscountViaPercent
                            }
                        };

                        var calcResult = _calculateOrderPositionCostService.CalculateOrderPositionCost(orderInfo.OrderType,
                                                                                                       orderInfo.ReleaseCountFact,
                                                                                                       pricePositionInfo.PriceId,
                                                                                                       orderInfo.SourceOrganizationUnitId,
                                                                                                       orderInfo.DestOrganizationUnitId,
                                                                                                       orderInfo.FirmId,
                                                                                                       request.CategoryIds,
                                                                                                       positionInfo);

                        orderPosition.CategoryRate = calcResult.Rate;
                        orderPosition.ShipmentPlan = calcResult.ShipmentPlan;
                        orderPosition.PricePerUnit = calcResult.PricePerUnit;
                        orderPosition.PayablePrice = calcResult.PayablePriceWithoutVat;
                        orderPosition.PricePerUnitWithVat = calcResult.PricePerUnitWithVat;
                        orderPosition.PayablePlan = calcResult.PayablePlan;
                        orderPosition.PayablePlanWoVat = calcResult.PayablePlanWoVat;
                        orderPosition.DiscountPercent = calcResult.DiscountPercent;
                        orderPosition.DiscountSum = calcResult.DiscountSum;
                    }
                    else
                    {
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
                    }

                    ValidateEntity(orderPosition, pricePositionInfo.AccountingMethodEnum == PositionAccountingMethod.PlannedProvision);

                    // Сохраняем изменения OrderPosition в БД
                    _orderRepository.CreateOrUpdate(orderPosition);
                    if (!isOrderPositionAlreadyExisting)
                    {
                        _replaceOrderPositionAdvertisementLinksOperationService.Replace(orderPosition.Id, advertisementsLinks);
                    }

                    var order = _orderReadModel.GetOrderSecure(orderPosition.OrderId);

                    _publicService.Handle(new UpdateOrderFinancialPerformanceRequest { Order = order, ReleaseCountFact = orderInfo.ReleaseCountFact });
                    _publicService.Handle(new ActualizeOrderReleaseWithdrawalsRequest { Order = order });

                    var targetPlatformId = _orderReadModel.EvaluateOrderPlatformId(order.Id);
                    var evaluatedOrderNumbersInfo = _orderReadModel.EvaluateOrderNumbers(order.Number, order.RegionalNumber, targetPlatformId);

                    order.PlatformId = targetPlatformId;
                    order.Number = evaluatedOrderNumbersInfo.Number;
                    order.RegionalNumber = evaluatedOrderNumbersInfo.RegionalNumber;
                    _orderRepository.Update(order);
                    scope.Updated<Order>(order.Id);
                }

                if (isOrderPositionAlreadyExisting)
                {
                    scope
                        .Updated<OrderPosition>(orderPosition.Id);
                }
                else
                {
                    scope
                        .Added<OrderPosition>(orderPosition.Id);
                }

                scope.Complete();
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
    }
}

