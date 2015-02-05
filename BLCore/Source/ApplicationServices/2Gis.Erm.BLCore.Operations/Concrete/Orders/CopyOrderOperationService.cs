using System;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
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
    public class CopyOrderOperationService : ICopyOrderOperationService
    {
        private const EntityAccessTypes RequiredAccess = EntityAccessTypes.Create | EntityAccessTypes.Update;
        private readonly IUserContext _userContext;
        private readonly IPublicService _publicService;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IEvaluateOrderNumberService _numberService;

        public CopyOrderOperationService(IUserContext userContext,
                                         IPublicService publicService,
                                         ISecurityServiceEntityAccess securityServiceEntityAccess,
                                         IOrderRepository orderRepository,
                                         IOperationScopeFactory scopeFactory,
                                         IOrderReadModel orderReadModel,
                                         IEvaluateOrderNumberService numberService)
        {
            _userContext = userContext;
            _publicService = publicService;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _numberService = numberService;
        }

        public CopyOrderResult CopyOrder(long orderId, bool isTechnicalTermination)
        {
            var orderIndex = _orderRepository.GenerateNextOrderUniqueNumber();
            var orderToCopy = _orderReadModel.GetOrderSecure(orderId);
            var beginDistributionDate = DateTime.UtcNow.GetNextMonthFirstDate();
            return CopyOrder(orderToCopy, orderIndex, isTechnicalTermination, beginDistributionDate, orderToCopy.ReleaseCountPlan, DiscountType.Default, orderToCopy.DealId);
        }

        public CopyOrderResult CopyOrder(long orderId, DateTime beginDistibutionDate, short releaseCountPlan, DiscountType discountType, long newOrderDealId)
        {
            var orderIndex = _orderRepository.GenerateNextOrderUniqueNumber();
            var orderToCopy = _orderReadModel.GetOrderSecure(orderId);
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
                    if (orderToCopy.WorkflowStepId != OrderState.OnTermination)
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

                _publicService.Handle(new ActualizeOrderReleaseWithdrawalsRequest { Order = newOrder });

                newOrder.DealId = newOrderDealId;

                _orderRepository.Update(newOrder);

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

            var evaluatedOrderNumbersInfo = _orderReadModel.EvaluateOrderNumbers(orderNumber, orderRegionalNumber, order.PlatformId);
            order.Number = evaluatedOrderNumbersInfo.Number;
            order.RegionalNumber = evaluatedOrderNumbersInfo.RegionalNumber;

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
            var syncCodes = _orderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            return _numberService.Evaluate(order.Number, syncCodes[order.SourceOrganizationUnitId], syncCodes[order.DestOrganizationUnitId], reservedNumber);
        }

        private string GenerateRegionalOrderNumber(Order order, long reservedNumber)
        {
            var syncCodes = _orderReadModel.GetOrderOrganizationUnitsSyncCodes(order.SourceOrganizationUnitId, order.DestOrganizationUnitId);
            return _numberService.EvaluateRegional(order.Number, syncCodes[order.SourceOrganizationUnitId], syncCodes[order.DestOrganizationUnitId], reservedNumber);
        }
    }
}
