using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteOrderPositionService : IDeleteGenericEntityService<OrderPosition>
    {
        private readonly IFinder _finder;
        private readonly IOrderRepository _orderRepository;
        private readonly IPublicService _publicService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteOrderPositionService(IFinder finder, IOrderRepository orderRepository, IPublicService publicService, IUnitOfWork unitOfWork, IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _orderRepository = orderRepository;
            _publicService = publicService;
            _unitOfWork = unitOfWork;
            _scopeFactory = scopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var orderPositionInfo = _finder.Find(Specs.Find.ById<OrderPosition>(entityId))
                                           .Select(position => new
                                               {
                                                   OrderPosition = position,
                                                   position.Order,
                                                   OrderWorkflowStepId = position.Order.WorkflowStepId,
                                                   OrderDealId = position.Order.DealId,
                                                   OrderReleaseCountFact = position.Order.ReleaseCountFact
                                               })
                                           .SingleOrDefault();

            if (orderPositionInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            if (orderPositionInfo.OrderWorkflowStepId != (int)OrderState.OnRegistration)
            {
                throw new ArgumentException(BLResources.CannotRemoveOrderPositionOfRegisteredOrder);
            }

            var orderDiscounts = _orderRepository.GetOrderDiscounts(orderPositionInfo.Order.Id);
            using (var operationScope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPosition>())
            {
                _orderRepository.Delete(orderPositionInfo.OrderPosition);

                using (var scope = _unitOfWork.CreateScope())
                {
                    var scopedOrderRepository = scope.CreateRepository<IOrderRepository>();
                    var hasOtherActiveOrderPosition = _finder.Find<OrderPosition>(position => position.OrderId == orderPositionInfo.Order.Id &&
                                                                                              position.Id != entityId &&
                                                                                              !position.IsDeleted && position.IsActive)
                                                             .Any();
                    if (!hasOtherActiveOrderPosition)
                    {
                        // в случае, если в заказе не осталось активных позиций => пометить order.BudgetType = Undefined
                        orderPositionInfo.Order.BudgetType = (int)OrderBudgetType.None;
                    }


                    // определим платформу заказа
                    scopedOrderRepository.DetermineOrderPlatform(orderPositionInfo.Order);
                    scopedOrderRepository.UpdateOrderNumber(orderPositionInfo.Order);

                    _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                        {
                            Order = orderPositionInfo.Order,
                            RecalculateFromOrder = true,
                            OrderDiscountInPercents = orderDiscounts.CalculateDiscountViaPercent,
                            ReleaseCountFact = orderPositionInfo.OrderReleaseCountFact,
                        });

                    _publicService.Handle(new CalculateReleaseWithdrawalsRequest { Order = orderPositionInfo.Order });

                    if (orderPositionInfo.OrderDealId != null)
                    {
                        _publicService.Handle(new ActualizeDealProfitIndicatorsRequest { DealIds = new[] { orderPositionInfo.OrderDealId.Value } });
                    }

                    scopedOrderRepository.Update(orderPositionInfo.Order);
                    scope.Complete();
                }

                operationScope
                    .Deleted<OrderPosition>(entityId)
                    .Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var orderPositionInfo = _finder.Find(Specs.Find.ById<OrderPosition>(entityId))
                .Select(position => new
                                    {
                                        position.PricePosition.Position.Name,
                                        OrderWorkflowStepId = position.Order.WorkflowStepId,
                                    })
                .SingleOrDefault();

            if (orderPositionInfo == null)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityNotFound
                    };
            }

            if (orderPositionInfo.OrderWorkflowStepId != (int)OrderState.OnRegistration)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.CannotRemoveOrderPositionOfRegisteredOrder
                    };
            }

            return new DeleteConfirmationInfo
            {
                EntityCode = orderPositionInfo.Name,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}
