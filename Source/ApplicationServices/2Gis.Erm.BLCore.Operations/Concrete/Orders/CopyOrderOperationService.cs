using System;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Operations\Operations\Concrete\Orders
    public class CopyOrderOperationService : ICopyOrderOperationService
    {
        private const EntityAccessTypes RequiredAccess = EntityAccessTypes.Create | EntityAccessTypes.Update;
        private readonly IUserContext _userContext;
        private readonly IPublicService _publicService;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealActualizeDealProfitIndicatorsAggregateService _dealActualizeDealProfitIndicatorsAggregateService;

        public CopyOrderOperationService(IUserContext userContext,
                                         IPublicService publicService,
                                         ISecurityServiceEntityAccess securityServiceEntityAccess,
                                         IOrderRepository orderRepository,
                                         IOperationScopeFactory scopeFactory,
                                         IDealActualizeDealProfitIndicatorsAggregateService dealActualizeDealProfitIndicatorsAggregateService,
                                         IDealReadModel dealReadModel,
                                         IOrderReadModel orderReadModel)
        {
            _userContext = userContext;
            _publicService = publicService;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _dealActualizeDealProfitIndicatorsAggregateService = dealActualizeDealProfitIndicatorsAggregateService;
            _dealReadModel = dealReadModel;
            _orderReadModel = orderReadModel;
        }

        public CopyOrderResult CopyOrder(long orderId, bool isTechnicalTermination)
        {
            var orderIndex = _orderRepository.GenerateNextOrderUniqueNumber();
            var orderToCopy = _orderReadModel.GetOrder(orderId);
            var beginDistributionDate = DateTime.UtcNow.GetNextMonthFirstDate();
            return CopyOrder(orderToCopy, orderIndex, isTechnicalTermination, beginDistributionDate, orderToCopy.ReleaseCountPlan, DiscountType.Default, orderToCopy.DealId);
        }

        public CopyOrderResult CopyOrder(long orderId, DateTime beginDistibutionDate, short releaseCountPlan, DiscountType discountType, long newOrderDealId)
        {
            var orderIndex = _orderRepository.GenerateNextOrderUniqueNumber();
            var orderToCopy = _orderReadModel.GetOrder(orderId);
            return CopyOrder(orderToCopy, orderIndex, false, beginDistibutionDate, releaseCountPlan, discountType, newOrderDealId);
        }

        private CopyOrderResult CopyOrder(Order orderToCopy, long orderIndex, bool isTechnicalTermination, DateTime beginDistibutionDate, short releaseCount, DiscountType discountType, long? newOrderDealId)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<CopyOrderIdentity>())
            {
                var hasAccess = _securityServiceEntityAccess.HasEntityAccess(RequiredAccess,
                                                                             EntityName.Order,
                                                                             _userContext.Identity.Code,
                                                                             orderToCopy.Id,
                                                                             orderToCopy.OwnerCode,
                                                                             null);
                if (!hasAccess)
                {
                    throw new SecurityException(BLResources.UserIsNotAllowedToCopyOrder);
                }

                if (isTechnicalTermination)
                {
                    if (orderToCopy.WorkflowStepId != (int)OrderState.OnTermination)
                    {
                        throw new InvalidOperationException(BLResources.CannotCreateOderInsteadOfTerminatedIfSourceOrderIsnNotOnTermination);
                    }
                }

                var orderPositionDtos = _orderReadModel.GetOrderPositionsWithAdvertisements(orderToCopy.Id);

                // FIXME {all, 11.12.2013}: реализация метода CreateCopiedOrder изменяет входной order. Нужно от этого избавиться, например запользовать внутри Omu.ValueInjecter
                var orderToCopyId = orderToCopy.Id;
                var newOrder = _orderRepository.CreateCopiedOrder(orderToCopy, orderPositionDtos);

                AdjustOrderValues(orderToCopyId, orderIndex, isTechnicalTermination, newOrder, beginDistibutionDate, releaseCount);

                var discountInPercents = discountType == DiscountType.Default 
                    ? IsDiscountInPercents(orderPositionDtos)
                    : discountType == DiscountType.InPercents;

                var recalculateResponse = (RecalculateOrderDiscountResponse)_publicService.Handle(new RecalculateOrderDiscountRequest
                {
                    OrderId = newOrder.Id,
                    ReleaseCountFact = newOrder.ReleaseCountFact,

                    InPercents = discountInPercents,
                    DiscountSum = newOrder.DiscountSum ?? 0m,
                    DiscountPercent = newOrder.DiscountPercent ?? 0m,
                });

                newOrder.DiscountSum = recalculateResponse.CorrectedDiscountSum;
                newOrder.DiscountPercent = recalculateResponse.CorrectedDiscountPercent;

                _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                {
                    Order = newOrder,
                    ReleaseCountFact = newOrder.ReleaseCountFact,

                    RecalculateFromOrder = true,
                    OrderDiscountInPercents = discountInPercents,
                });

                _publicService.Handle(new CalculateReleaseWithdrawalsRequest { Order = newOrder });

                newOrder.DealId = newOrderDealId;

                _orderRepository.Update(newOrder);

                if (newOrder.DealId != null)
                {
                    // TODO {all, 04.11.2013}: при рефакторинге ApplicationServices для обеспечения SRP - проверить есть ли реальная необходимость в отдельном режиме без обработки deal.ActualProfit - если нет выпилить флаг из сигнатуры
                    var dealInfos = _dealReadModel.GetInfoForActualizeProfits(new[] { newOrder.DealId.Value }, false);
                    _dealActualizeDealProfitIndicatorsAggregateService.Actualize(dealInfos);
                }

                operationScope
                    .Updated<Order>(newOrder.Id)
                    .Complete();

                return new CopyOrderResult { OrderId = newOrder.Id, OrderNumber = newOrder.Number };
            }
        }

        private static bool IsDiscountInPercents(OrderPositionWithAdvertisementsDto[] orderPositionDtos)
        {
            return orderPositionDtos
                .Where(y => !y.OrderPosition.IsDeleted && y.OrderPosition.IsActive)
                .All(y => y.OrderPosition.CalculateDiscountViaPercent);
        }

        private void AdjustOrderValues(
            long orderId,
            long orderIndex,
            bool isTechnicalTermination,
            Order order, 
            DateTime rawBeginDistributionDate,
            short releaseCount)
        {
            // Прежняя логика была построена на том, что сначала номера генерируются, а затем присваиваются. Возможно, это важно.
            var orderNumber = GenerateOrderNumber(order, orderIndex);
            var orderRegionalNumber = IsBranchToBranchOrder(order) ? GenerateRegionalOrderNumber(order, orderIndex) : null;
            order.Number = orderNumber;
            order.RegionalNumber = orderRegionalNumber;

            _orderRepository.UpdateOrderNumber(order);

            order.PayableFact = order.PayablePlan;
            order.ReleaseCountPlan = releaseCount;
            order.ReleaseCountFact = releaseCount;

            // Если заказ содаётся вместо расторгнутого по техническим причинам
            order.TechnicallyTerminatedOrderId = isTechnicalTermination ? (long?)orderId : null;

            // releases
            

            var releaseNumbersDto = _orderReadModel.CalculateReleaseNumbers(order.DestOrganizationUnitId,
                                                                             rawBeginDistributionDate,
                                                                             order.ReleaseCountPlan,
                                                                             order.ReleaseCountFact);
            order.BeginReleaseNumber = releaseNumbersDto.BeginReleaseNumber;
            order.EndReleaseNumberPlan = releaseNumbersDto.EndReleaseNumberPlan;
            order.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;

            var distributionDatesDto = _orderReadModel.CalculateDistributionDates(rawBeginDistributionDate, order.ReleaseCountPlan, order.ReleaseCountFact);
            order.BeginDistributionDate = distributionDatesDto.BeginDistributionDate;
            order.EndDistributionDatePlan = distributionDatesDto.EndDistributionDatePlan;
            order.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;
        }

        private bool IsBranchToBranchOrder(Order order)
        {
            return order.SourceOrganizationUnitId != order.DestOrganizationUnitId && !_orderReadModel.IsBranchToBranchOrder(order);
        }

        private string GenerateOrderNumber(Order order, long reservedNumber)
        {
            var request = new GenerateOrderNumberRequest
                {
                    Order = order,
                    ReservedNumber = reservedNumber
                };
            var response = (GenerateOrderNumberResponse)_publicService.Handle(request);
            return response.Number;
        }

        private string GenerateRegionalOrderNumber(Order order, long reservedNumber)
        {
            var generateOrderRegionalNumberRequest = new GenerateOrderNumberRequest
                {
                    Order = order,
                    ReservedNumber = reservedNumber,
                    IsRegionalNumber = true
                };
            var generateOrderRegionalNumberResponse = (GenerateOrderNumberResponse)_publicService.Handle(generateOrderRegionalNumberRequest);
            return generateOrderRegionalNumberResponse.Number;
        }
    }
}
