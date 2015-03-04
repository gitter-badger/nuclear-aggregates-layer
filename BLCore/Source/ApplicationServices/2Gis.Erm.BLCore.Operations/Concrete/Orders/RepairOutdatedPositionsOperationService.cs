using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class RepairOutdatedPositionsOperationService : IRepairOutdatedPositionsOperationService
    {
        private readonly ICommonLog _logger;
        private readonly IPublicService _publicService;

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IValidateOrderPositionAdvertisementsOperationService _validateOrderPositionAdvertisementsOperationService;
        private readonly IOrderDeleteReleaseWithdrawalsAggregateService _deleteReleaseWithdrawalsAggregateService;
        private readonly IOrderDeleteReleaseTotalsAggregateService _deleteReleaseTotalsAggregateService;

        public RepairOutdatedPositionsOperationService(
            IOrderReadModel orderReadModel,
            IPositionReadModel positionReadModel,
            IPriceReadModel priceReadModel,
            IPublicService publicService,
            IOrderRepository orderRepository,
            IValidateOrderPositionAdvertisementsOperationService validateOrderPositionAdvertisementsOperationService,
            IOrderDeleteReleaseWithdrawalsAggregateService deleteReleaseWithdrawalsAggregateService,
            IOrderDeleteReleaseTotalsAggregateService deleteReleaseTotalsAggregateService,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _logger = logger;
            _publicService = publicService;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _validateOrderPositionAdvertisementsOperationService = validateOrderPositionAdvertisementsOperationService;
            _deleteReleaseWithdrawalsAggregateService = deleteReleaseWithdrawalsAggregateService;
            _deleteReleaseTotalsAggregateService = deleteReleaseTotalsAggregateService;
            _orderReadModel = orderReadModel;
            _positionReadModel = positionReadModel;
            _priceReadModel = priceReadModel;
        }

        public IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId)
        {
            return RepairOutdatedPositions(orderId, false);
        }

        public IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId, bool saveDiscounts)
        {
            var resultMessages = new List<RepairOutdatedPositionsOperationMessage>();

            using (var scope = _scopeFactory.CreateNonCoupled<RepairOutdatedIdentity>())
            {
                long actualPriceId;
                if (!_orderReadModel.TryGetActualPriceIdForOrder(orderId, out actualPriceId))
                {
                    throw new BusinessLogicException(BLResources.OrderCheckOrderPositionsDoesntCorrespontToActualPrice);
                }

                var currentOrderInfo = _orderReadModel.GetOrderInfoForRepairOutdatedPositions(orderId);
                if (currentOrderInfo.OrderPositions.All(orderPosition => orderPosition.PricePosition.PriceId == actualPriceId))
                {   // все позиции заказа актуальны => актуализация не требуется
                    scope.Complete();
                    return resultMessages;
                }

                // 1: удаление всего.
                RemoveOutdated(currentOrderInfo);

                // 2: сохранение актуализированных данных.
                ActualizeOrderPositions(currentOrderInfo.OrderPositions, actualPriceId, resultMessages, saveDiscounts);
                
                var order = _orderReadModel.GetOrderSecure(orderId);
                _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                    {
                        Order = order,
                        OrderDiscountInPercents = true,
                        RecalculateFromOrder = false,
                        ReleaseCountFact = order.ReleaseCountFact
                    });

                _orderRepository.Update(order);

                scope.Updated<Order>(orderId)
                     .Complete();
            }

            return resultMessages;
        }

        private static bool AmountSpecificationModeChangedToFixedValue(PricePosition outdatedPricePosition, PricePosition actualPricePosition)
        {
            var outdatedAmountMode = outdatedPricePosition.AmountSpecificationMode;
            var actualAmountMode = actualPricePosition.AmountSpecificationMode;
            return outdatedAmountMode != PricePositionAmountSpecificationMode.FixedValue &&
                   actualAmountMode == PricePositionAmountSpecificationMode.FixedValue;
        }

        private static int GetPositionAmount(OrderPosition orderPosition, PricePosition pricePosition, int adverisementCount)
        {
            var actualAmountSpecificationMode = pricePosition.AmountSpecificationMode;
            switch (actualAmountSpecificationMode)
            {
                case PricePositionAmountSpecificationMode.FixedValue:
                    if (!pricePosition.Amount.HasValue)
                    {
                        throw new NotificationException(BLResources.CannotCalculateOrderPositionAmount);
                    }

                    return pricePosition.Amount.Value;
                case PricePositionAmountSpecificationMode.ArbitraryValue:
                    return orderPosition.Amount;
                case PricePositionAmountSpecificationMode.Counting:
                    return adverisementCount;
                default:
                    throw new NotificationException(string.Format(BLResources.NotSupportedEnumerationValue, pricePosition.AmountSpecificationMode));
            }
        }

        private static void AddWarningMessage(string message, IList<RepairOutdatedPositionsOperationMessage> messages)
        {
            messages.Add(new RepairOutdatedPositionsOperationMessage { MessageText = message, Type = API.Operations.Metadata.MessageType.Warning });
        }

        private bool CheckOrderPositionAdvertisementCorrectness(
            long orderPositionId,
            string positionName,
            IEnumerable<AdvertisementDescriptor> advertisements,
            IList<RepairOutdatedPositionsOperationMessage> resultMessages)
        {
            var validationErrors = _validateOrderPositionAdvertisementsOperationService.Validate(orderPositionId, advertisements);

            if (validationErrors.Any(x =>
                                      x.Rule != OrderPositionAdvertisementValidationRule.RequiredAdvertisement &&
                                      x.Rule != OrderPositionAdvertisementValidationRule.AdvertisementTemplateMatchesPositionTemplate))
            {
                AddWarningMessage(string.Format(BLResources.OutdatedPositionHasInvalidAdvertisement, positionName), resultMessages);
                return false;
            }

            var orderPositionAdvertisementsToUseDummy =
                validationErrors.Where(x => x.Rule == OrderPositionAdvertisementValidationRule.RequiredAdvertisement ||
                                            x.Rule == OrderPositionAdvertisementValidationRule.AdvertisementTemplateMatchesPositionTemplate)
                                .Select(x => x.Advertisement)
                                .ToArray();

            if (orderPositionAdvertisementsToUseDummy.Any(x => !x.DummyAdvertisementId.HasValue))
            {
                AddWarningMessage(string.Format(BLResources.OutdatedPositionDoesntHaveDummyAdvertisement, positionName), resultMessages);
                return false;
            }

            foreach (var advertisementDescriptor in orderPositionAdvertisementsToUseDummy)
            {
                advertisementDescriptor.AdvertisementId = advertisementDescriptor.DummyAdvertisementId;
            }

            return true;
        }

        private void ActualizeOrderPositions(IEnumerable<OrderRepairOutdatedOrderPositionDto.OrderPositionDto> currentOrderPositions,
                                             long actualPriceId,
                                             List<RepairOutdatedPositionsOperationMessage> resultMessages,
                                             bool saveDiscounts)
        {
            foreach (var orderPosition in currentOrderPositions)
            {
                var advertisements = orderPosition.ClonedAdvertisements.ToList();

                if (CheckOrderPositionAdvertisementCorrectness(orderPosition.OrderPosition.Id, orderPosition.PositionName, advertisements, resultMessages))
                {
                    if (orderPosition.PricePosition.PriceId == actualPriceId)
                    {
                        RestoreClonedOrderPosition(orderPosition.OrderPosition,
                                                    orderPosition.ClonedAdvertisements.ToList(),
                                                    orderPosition.PricePosition,
                                                    saveDiscounts);
                        continue;
                    }
                    
                    resultMessages.AddRange(UpgradeAndRestoreOrderPosition(orderPosition.OrderPosition,
                                                                                advertisements,
                                                                                orderPosition.PricePosition,
                                                                                actualPriceId,
                                                                                saveDiscounts));
                }
            }
        }

        private void RemoveOutdated(OrderRepairOutdatedOrderPositionDto currenOrderInfo)
        {
            foreach (var orderPosition in currenOrderInfo.OrderPositions)
            {
                _deleteReleaseWithdrawalsAggregateService.Delete(orderPosition.ReleaseWithdrawals);

                _orderRepository.Delete(orderPosition.Advertisements);
                _orderRepository.Delete(orderPosition.OrderPosition);
            }

            _deleteReleaseTotalsAggregateService.Delete(currenOrderInfo.ReleaseTotals);
        }

        private IEnumerable<RepairOutdatedPositionsOperationMessage> UpgradeAndRestoreOrderPosition(OrderPosition orderPosition,
                                                                                                    List<AdvertisementDescriptor> advertisements,
                                                                                                    PricePosition pricePosition,
                                                                                                    long actualPriceId,
                                                                                                    bool saveDiscount)
        {
            var resultMessages = new List<RepairOutdatedPositionsOperationMessage>();

            var positionName = _positionReadModel.GetPositionName(pricePosition.PositionId);
            var actualPricePosition = _priceReadModel.GetPricePosition(actualPriceId, pricePosition.PositionId);
            if (actualPricePosition == null)
            {
                var message = string.Format(BLResources.OrderPositionWasRemoved, positionName);
                _logger.InfoFormat(message);
                AddWarningMessage(message, resultMessages);
                return resultMessages;
            }

            // В случае изменения в направлении
            // * свободное -> фиксированное значение,
            // * по числу РМ -> фиксированное значение 
            // все рекламные позиции удаляются.
            if (AmountSpecificationModeChangedToFixedValue(pricePosition, actualPricePosition))
            {
                // В случае, если требуется число рекламных позиций, отличное от того, что имеется - сделать ничего не можем, 
                // так как если сохраним с неверным числом рекламных материалов позиция заказа будет проходить проверки, но по сути станет невалидной.
                // Поэтому восстанавливаем позицию из старого прайса и сообщаем пользователю, что ему придётся поработать ручками.
                var needAdvCount = GetPositionAmount(orderPosition, actualPricePosition, advertisements.Count);
                if (advertisements.Count != needAdvCount)
                {
                    AddWarningMessage(string.Format(BLResources.OrderPositionWasNotUpgraded, positionName), resultMessages);
                    RestoreClonedOrderPosition(orderPosition, advertisements, pricePosition, saveDiscount);
                    return resultMessages;
                }
            }

            _logger.InfoFormat(BLResources.OrderPositionWasReplaced, positionName, orderPosition.OrderId, actualPricePosition.PriceId);
            RestoreClonedOrderPosition(orderPosition, advertisements, actualPricePosition, saveDiscount);
            return resultMessages;
        }

        private void RestoreClonedOrderPosition(OrderPosition orderPosition,
                                                List<AdvertisementDescriptor> orderPositionAdvertisements,
                                                PricePosition pricePosition,
                                                bool saveDiscount)
        {
            // Нельзя использовать то значение, что было в устаревшей позиции заказа, поскольку способ исчисления мог измениться (см. GetPositionAmount)
            var amount = GetPositionAmount(orderPosition, pricePosition, orderPositionAdvertisements.Count);

            var newPosition = new OrderPosition
                {
                    OrderId = orderPosition.OrderId,
                    Amount = amount,
                    PricePositionId = pricePosition.Id,

                    Comment = orderPosition.Comment,
                    CalculateDiscountViaPercent = true,
                    DiscountPercent = saveDiscount ? orderPosition.DiscountPercent : 0m,

                    IsActive = true
                };

            var createRequest = new EditOrderPositionRequest
                {
                    Entity = newPosition,
                    HasUpdatedAdvertisementMaterials = true,
                    AdvertisementsLinks = orderPositionAdvertisements.ToArray(),
                };
            _publicService.Handle(createRequest);
        }
    }
}
