using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteOrderPositionOperationService : IDeleteGenericEntityService<OrderPosition>
    {
        private readonly IFinder _finder;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderRepository _orderRepository;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteOrderPositionOperationService(
            IFinder finder,
            IOrderRepository orderRepository,
            IPublicService publicService,
            IOperationScopeFactory scopeFactory,
            IOrderReadModel orderReadModel)
        {
            _finder = finder;
            _orderRepository = orderRepository;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var orderPositionId = entityId;

            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPosition>())
            {
                var orderPositionDeleteInfo = _orderReadModel.GetOrderPositionDeleteInfo(orderPositionId);
                if (orderPositionDeleteInfo == null)
                {
                    throw new BusinessLogicException(BLResources.EntityNotFound);
                }

                if (orderPositionDeleteInfo.Order.WorkflowStepId != OrderState.OnRegistration)
                {
                    throw new BusinessLogicException(BLResources.CannotRemoveOrderPositionOfRegisteredOrder);
                }

                _orderRepository.Delete(orderPositionDeleteInfo.OrderPosition);
                
                _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                        {
                            Order = orderPositionDeleteInfo.Order,
                            RecalculateFromOrder = true,
                            OrderDiscountInPercents = orderPositionDeleteInfo.IsDiscountViaPercentCalculation,
                            ReleaseCountFact = orderPositionDeleteInfo.Order.ReleaseCountFact,
                        });

                _publicService.Handle(new ActualizeOrderReleaseWithdrawalsRequest { Order = orderPositionDeleteInfo.Order });

                var targetPlatformId = _orderReadModel.EvaluateOrderPlatformId(orderPositionDeleteInfo.Order.Id);
                var evaluatedOrderNumbersInfo = _orderReadModel.EvaluateOrderNumbers(orderPositionDeleteInfo.Order.Number,
                                                                                        orderPositionDeleteInfo.Order.RegionalNumber,
                                                                                        targetPlatformId);

                orderPositionDeleteInfo.Order.PlatformId = targetPlatformId;
                orderPositionDeleteInfo.Order.Number = evaluatedOrderNumbersInfo.Number;
                orderPositionDeleteInfo.Order.RegionalNumber = evaluatedOrderNumbersInfo.RegionalNumber;
                _orderRepository.Update(orderPositionDeleteInfo.Order);

                scope.Deleted<OrderPosition>(orderPositionId)
                     .Updated<Order>(orderPositionDeleteInfo.Order.Id)
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

            if (orderPositionInfo.OrderWorkflowStepId != OrderState.OnRegistration)
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