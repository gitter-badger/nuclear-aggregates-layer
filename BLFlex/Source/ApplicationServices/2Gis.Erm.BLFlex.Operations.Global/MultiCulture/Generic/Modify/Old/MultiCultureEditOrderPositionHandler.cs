using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.Old
{
    // FIXME {all, 10.07.2014}: почти полная copy/paste других adapted версий этого handler, при рефакторинге ApplicationServices - попытаться объеденить обратно + использование finder и т.п.
    public sealed class MultiCultureEditOrderPositionHandler : RequestHandler<EditOrderPositionRequest, EmptyResponse>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        private readonly IPublicService _publicService;
        private readonly IOrderRepository _orderRepository;
        private readonly IReplaceOrderPositionAdvertisementLinksOperationService _replaceOrderPositionAdvertisementLinksOperationService;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;
        private readonly ICheckIfOrderPositionCanBeModifiedOperationService _checkIfOrderPositionCanBeModifiedOperationService;

        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICategoryReadModel _categoryReadModel;
        private readonly IPositionReadModel _positionReadModel;

        public MultiCultureEditOrderPositionHandler(IFinder finder,
                                                    IOrderReadModel orderReadModel,
                                                    IOrganizationUnitReadModel organizationUnitReadModel,
                                                    IPublicService publicService,
                                                    IOrderRepository orderRepository,
                                                    IReplaceOrderPositionAdvertisementLinksOperationService replaceOrderPositionAdvertisementLinksOperationService,
                                                    ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
                                                    IOperationScopeFactory scopeFactory,
                                                    ICategoryReadModel categoryReadModel,
                                                    ICheckIfOrderPositionCanBeModifiedOperationService checkIfOrderPositionCanBeModifiedOperationService,
                                                    IPositionReadModel positionReadModel)
        {
            _finder = finder;
            _orderReadModel = orderReadModel;
            _organizationUnitReadModel = organizationUnitReadModel;
            _publicService = publicService;
            _orderRepository = orderRepository;
            _replaceOrderPositionAdvertisementLinksOperationService = replaceOrderPositionAdvertisementLinksOperationService;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _scopeFactory = scopeFactory;
            _categoryReadModel = categoryReadModel;
            _checkIfOrderPositionCanBeModifiedOperationService = checkIfOrderPositionCanBeModifiedOperationService;
            _positionReadModel = positionReadModel;
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
                                                        OrderType = x.OrderType
                                                    })
                                   .Single();

            string checkReport;
            if (!_checkIfOrderPositionCanBeModifiedOperationService.Check(orderPosition.OrderId,
                                                                                 orderPosition.Id,
                                                                                 orderPosition.PricePositionId,
                                                                                 advertisementsLinks,
                                                                                 out checkReport))
            {
                throw new NotificationException(string.Format(BLResources.CannotCreateOrderPositionTemplate, checkReport));
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
                                                   .Select(x => new
                                                                    {
                                                                        x.Cost,
                                                                        SalesModel = x.Position.SalesModel,
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
                        var positionIds = request.AdvertisementsLinks.Select(x => x.PositionId).Distinct().ToArray();
                        var positionGroups = _positionReadModel.GetPositionGroups(positionIds);
                        var mediaPositions = positionGroups.Where(x => x.Value == PositionsGroup.Media).Select(x => x.Key).ToArray();
                        var categoriesToCheck =
                            request.AdvertisementsLinks.Where(x => x.CategoryId.HasValue && !mediaPositions.Contains(x.PositionId))
                                   .Select(x => x.CategoryId.Value)
                                   .Distinct()
                                   .ToArray();

                        var unsupported = _categoryReadModel.PickCategoriesUnsupportedBySalesModelInOrganizationUnit(pricePositionInfo.SalesModel,
                                                                                      orderInfo.DestOrganizationUnitId,
                                                                                      categoriesToCheck);
                        if (unsupported.Any())
                        {
                            var organizationUnitName = _organizationUnitReadModel.GetName(orderInfo.DestOrganizationUnitId);
                            throw new CategoryIsRestrictedBySalesModelException(unsupported.Select(pair => pair.Value), organizationUnitName, pricePositionInfo.SalesModel);
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

                    orderPosition.CategoryRate = categoryRate;
                    orderPosition.ShipmentPlan = calculateOrderPositionPricesResponse.ShipmentPlan;
                    orderPosition.PricePerUnit = calculateOrderPositionPricesResponse.PricePerUnit;
                    orderPosition.PayablePrice = calculateOrderPositionPricesResponse.PayablePrice;
                    orderPosition.PricePerUnitWithVat = calculateOrderPositionPricesResponse.PricePerUnitWithVat;
                    orderPosition.PayablePlan = calculateOrderPositionPricesResponse.PayablePlan;
                    orderPosition.PayablePlanWoVat = calculateOrderPositionPricesResponse.PayablePlanWoVat;
                    orderPosition.DiscountPercent = calculateOrderPositionPricesResponse.DiscountPercent;
                    orderPosition.DiscountSum = calculateOrderPositionPricesResponse.DiscountSum;

                    ValidateEntity(orderPosition, pricePositionInfo.SalesModel);

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
                    scope.Updated<OrderPosition>(orderPosition.Id);
                }
                else
                {
                    scope.Added<OrderPosition>(orderPosition.Id);
                }

                scope.Complete();
            }

            return Response.Empty;
        }

        private static void ValidateEntity(OrderPosition entity, SalesModel salesModel)
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

            if (entity.PricePerUnitWithVat < 0 || entity.PricePerUnitWithVat < 0)
            {
                throw new NotificationException(BLResources.AttemptToSaveBudgeteOrderPositionWithNegativePrice);
            }

            if (salesModel == SalesModel.PlannedProvision && entity.Amount != 1)
            {
                throw new NotificationException(string.Format(BLResources.AttemptToSaveOrderPositionWithAmountNotEqualToOne,
                                                              salesModel.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));
            }
        }
    }
}