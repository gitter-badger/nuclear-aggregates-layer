using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class CloseOrderHandler : RequestHandler<CloseOrderRequest, EmptyResponse>
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;

        public CloseOrderHandler(
            IDealReadModel dealReadModel,
            IDealChangeStageAggregateService dealChangeStageAggregateService,
            ISubRequestProcessor subRequestProcessor, 
            IOperationScopeFactory scopeFactory,
            IOrderRepository orderRepository,
            IOrderReadModel orderReadModel)
        {
            _dealReadModel = dealReadModel;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
            _subRequestProcessor = subRequestProcessor;
            _scopeFactory = scopeFactory;
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
        }

        protected override EmptyResponse Handle(CloseOrderRequest request)
        {
            var order = _orderReadModel.GetOrderSecure(request.OrderId);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), request.OrderId);
            }

            if (!(order.WorkflowStepId == OrderState.OnRegistration || order.WorkflowStepId == OrderState.Rejected))
            {
                throw new NotificationException(BLResources.OrderCloseIncorrectState);
            }

            if (!order.IsActive)
            {
                throw new NotificationException(BLResources.OrderHasAlreadyBeenClosed);
            }
            
            using (var operationScope = _scopeFactory.CreateNonCoupled<CloseWithDenialIdentity>())
            {
                _orderRepository.CloseOrder(order, request.Reason);

                _subRequestProcessor.HandleSubRequest(new ActualizeOrderReleaseWithdrawalsRequest { Order = order }, Context);
                _subRequestProcessor.HandleSubRequest(new UpdateOrderFinancialPerformanceRequest { Order = order, ReleaseCountFact = order.ReleaseCountFact }, Context);

                _orderRepository.Update(order);

                // Обновить сделку
                if (order.DealId.HasValue)
                {   
                    // У заказа есть связанный документ "Сделка" + Единственный заказ по сделке - > Изменить этап сделки на "Согласование сроков принятия решения"
                    var hasOtherOrdersInDeal =
                        _orderReadModel.GetOrdersForDeal(order.DealId.Value).Any(x => x.Id != request.OrderId);
                    if (!hasOtherOrdersInDeal)
                    {
                        var deal = _dealReadModel.GetDeal(order.DealId.Value);
                        _dealChangeStageAggregateService.ChangeStage(new[] { new DealChangeStageDto { Deal = deal, NextStage = DealStage.MatchAndSendProposition } });
                    }
                }

                operationScope
                    .Updated<Order>(request.OrderId)
                    .Complete();
            }

            return Response.Empty;
        }
    }
}
