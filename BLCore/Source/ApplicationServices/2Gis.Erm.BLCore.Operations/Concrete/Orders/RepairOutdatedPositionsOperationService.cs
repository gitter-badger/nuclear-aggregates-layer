using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders
{
    public class RepairOutdatedPositionsOperationService : IRepairOutdatedPositionsOperationService
    {
        private readonly ICommonLog _logger;
        private readonly IPublicService _publicService;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFinder _finder;
        private readonly IValidateOrderPositionAdvertisementsService _validateOrderPositionAdvertisementsService;

        public RepairOutdatedPositionsOperationService(
            ICommonLog logger,
            IPublicService publicService,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,

            // TODO {d.ivanov, 11.11.2013}: адаптировать под read-model
            IOperationScopeFactory scopeFactory,
            IFinder finder,
            IValidateOrderPositionAdvertisementsService validateOrderPositionAdvertisementsService,
            IOrderReadModel orderReadModel)
        {
            _logger = logger;
            _publicService = publicService;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _finder = finder;
            _validateOrderPositionAdvertisementsService = validateOrderPositionAdvertisementsService;
            _orderReadModel = orderReadModel;
        }

        public IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId)
        {
            return RepairOutdatedPositions(orderId, false);
        }

        public IEnumerable<RepairOutdatedPositionsOperationMessage> RepairOutdatedPositions(long orderId, bool saveDiscounts)
        {
            // Сначала вне scope проверяем, требуется ли заказу обновление.
            // поскольку эта операция только на чтение, внутри scope её делать не стоит, так как
            // иначе придется его закрывать, не сделав изменений в базе.     
            List<RepairOutdatedPositionsOperationMessage> resultMessages;
            IEnumerable<RepairOutdatedOrderPositionDto> currentOrderPositions;
            if (!EnsureActualization(orderId, out resultMessages, out currentOrderPositions))
            {
                return resultMessages;
            }

            using (var operationScope = _scopeFactory.CreateNonCoupled<RepairOutdatedIdentity>())
            {
                // Теперь стало ясно, что изменений не избежать и присиупаем к первой части: удаление всего.
                RemoveOutdatedData(orderId, currentOrderPositions);

                // Вторая часть: сохранение актуализорованных данных.
                long actualPriceId;
                _orderReadModel.TryGetActualPriceIdForOrder(orderId, out actualPriceId);
                ActualizeOrderPositions(currentOrderPositions, actualPriceId, resultMessages, saveDiscounts);
                var order = _orderReadModel.GetOrderSecure(orderId);
                _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                    {
                        Order = order,
                        OrderDiscountInPercents = true,
                        RecalculateFromOrder = false,
                        ReleaseCountFact = order.ReleaseCountFact
                    });

                _orderRepository.Update(order);

                operationScope
                    .Updated<Order>(orderId)
                    .Complete();
            }

            return resultMessages.ToArray();
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
            messages.Add(new RepairOutdatedPositionsOperationMessage { MessageText = message, Type = DoubleGis.Erm.BLCore.API.Operations.Metadata.MessageType.Warning });
        }

        private bool
            CheckOrderPositionAdvertisementCorrectness(long orderPositionId,
                                                       string positionName,
                                                       IEnumerable<AdvertisementDescriptor> advertisements,
                                                       IList<RepairOutdatedPositionsOperationMessage> resultMessages)
        {
            var validationErrors = _validateOrderPositionAdvertisementsService.Validate(orderPositionId, advertisements);

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

        private void ActualizeOrderPositions(IEnumerable<RepairOutdatedOrderPositionDto> currentOrderPositions,
                                             long actualPriceId,
                                             List<RepairOutdatedPositionsOperationMessage> resultMessages,
                                             bool saveDiscounts)
        {
            using (var scope = _unitOfWork.CreateScope())
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
                        }
                        else
                        {
                            resultMessages.AddRange(UpgradeAndRestoreOrderPosition(orderPosition.OrderPosition,
                                                                                   advertisements,
                                                                                   orderPosition.PricePosition,
                                                                                   actualPriceId,
                                                                                   saveDiscounts));
                        }
                    }
                }

                scope.Complete();
            }
        }

        private void RemoveOutdatedData(long orderId, IEnumerable<RepairOutdatedOrderPositionDto> currenOrderPositionDtos)
        {
            using (var scope = _unitOfWork.CreateScope())
            {
                var releaseWithdrawalRepository = scope.CreateRepository<IWithdrawalInfoRepository>();
                releaseWithdrawalRepository.DeleteReleaseWithdrawalPositionsForOrder(orderId);

                scope.Complete();
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var orderRepository = scope.CreateRepository<IOrderRepository>();
                var releaseWithdrawalRepository = scope.CreateRepository<IWithdrawalInfoRepository>();

                foreach (var orderPosition in currenOrderPositionDtos)
                {
                    releaseWithdrawalRepository.Delete(orderPosition.Withdrawals);

                    // ReleaseWithdrawalRepository напрашивается на объединение с OrderRepository
                    orderRepository.Delete(orderPosition.Advertisements);
                    orderRepository.Delete(orderPosition.OrderPosition);
                }

                orderRepository.DeleteOrderReleaseTotalsForOrder(orderId);

                scope.Complete();
            }
        }

        private bool EnsureActualization(long orderId,
                                         out List<RepairOutdatedPositionsOperationMessage> resultMessages,
                                         out IEnumerable<RepairOutdatedOrderPositionDto> currentOrderPositions)
        {
            long actualPriceId;
            resultMessages = new List<RepairOutdatedPositionsOperationMessage>();

            currentOrderPositions = GetOrderPositionsExtended(orderId);
            if (!_orderReadModel.TryGetActualPriceIdForOrder(orderId, out actualPriceId))
            {
                throw new NotificationException(BLResources.OrderCheckOrderPositionsDoesntCorrespontToActualPrice);
            }

            var allPositionsAreUpToDate = currentOrderPositions.All(orderPosition => orderPosition.PricePosition.PriceId == actualPriceId);
            return !allPositionsAreUpToDate;
        }

        private IEnumerable<RepairOutdatedPositionsOperationMessage> UpgradeAndRestoreOrderPosition(OrderPosition orderPosition,
                                                                                                    List<AdvertisementDescriptor> advertisements,
                                                                                                    PricePosition pricePosition,
                                                                                                    long actualPriceId,
                                                                                                    bool saveDiscount)
        {
            var resultMessages = new List<RepairOutdatedPositionsOperationMessage>();

            var actualPricePosition = GetPricePosition(pricePosition.PositionId, actualPriceId);
            var positionName = GetPositionName(pricePosition.PositionId);

            if (actualPricePosition == null)
            {
                var message = string.Format(BLResources.OrderPositionWasRemoved, positionName);
                _logger.InfoFormatEx(message);
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

            _logger.InfoFormatEx(BLResources.OrderPositionWasReplaced, positionName, orderPosition.OrderId, actualPricePosition.PriceId);
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

        // TODO {d.ivanov, 11.11.2013}: перенести в read-model
        private PricePosition GetPricePosition(long positionId, long priceId)
        {
            return _finder.Find(PriceSpecs.PricePositions.Find.ByPriceAndPosition(priceId, positionId))
                          .Where(Specs.Find.ActiveAndNotDeleted<PricePosition>())
                          .SingleOrDefault();
        }

        // TODO {d.ivanov, 11.11.2013}: перенести в read-model
        private string GetPositionName(long positionId)
        {
            return _finder.Find((Specs.Find.ById<Position>(positionId)))
                          .Select(item => item.Name)
                          .SingleOrDefault();
        }

        // TODO {d.ivanov, 11.11.2013}: перенести в read-model
        private IEnumerable<RepairOutdatedOrderPositionDto> GetOrderPositionsExtended(long orderId)
        {
            return _finder.Find<OrderPosition, RepairOutdatedOrderPositionDto>(OrderSpecs.OrderPositions.Select.RepairOutdatedOrderPositions(),
                                                                               OrderSpecs.OrderPositions.Find.ByOrder(orderId) &&
                                                                               Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .ToArray();
        }
    }
}
