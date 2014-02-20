using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.OrderValidation.AssociatedAndDeniedPositions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class OrderValidationOnStateChangeService : IOrderValidationOnStateChangeService
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IOrderReadModel _orderReadModel;

        public OrderValidationOnStateChangeService(ISubRequestProcessor subRequestProcessor,
                                                   IOrderValidationService orderValidationService,
                                                   IOrderReadModel orderReadModel)
        {
            _subRequestProcessor = subRequestProcessor;
            _orderValidationService = orderValidationService;
            _orderReadModel = orderReadModel;
        }

        public ValidateOrdersResult Validate(long orderId, OrderState newState, OrderValidationPredicate orderValidationPredicate, ValidateOrdersRequest validateOrdersRequest)
        {
            var previousState = _orderReadModel.GetOrderState(orderId);

            ValidateOrdersResponse checkResponse = null;

            if (newState != previousState)
            {
                if (newState == OrderState.OnApproval)
                {
                    return _orderValidationService.ValidateOrders(orderValidationPredicate, validateOrdersRequest);
                }

                var checkRequest = CreateSingleCheckRequest(newState, previousState, orderId);
                checkResponse = checkRequest != null
                                    ? (ValidateOrdersResponse)_subRequestProcessor.HandleSubRequest(checkRequest, null)
                                    : null;
            }

            return checkResponse == null
                       ? new ValidateOrdersResult { Messages = new OrderValidationMessage[0], OrderCount = 1 }
                       : new ValidateOrdersResult { Messages = checkResponse.Messages.ToArray(), OrderCount = 1 };
        }

        private CheckOrdersAssociatedAndDeniedPositionsRequest CreateSingleCheckRequest(OrderState newState, OrderState previousState, long orderId)
        {
            // FIXME {d.ivanov, 29.08.2013}: вызывается и из Web точки входа - мутный case, особенно, учитывая выделени данных для проверок в отдельный persistence
            // comment {all, 2013-10-10}: логика переехала из хендлера и более не вызывается web-приложением
            if (newState == OrderState.OnRegistration && (previousState == OrderState.OnApproval || previousState == OrderState.Approved))
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.OnTermination && previousState == OrderState.Approved)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.Rejected && previousState == OrderState.OnApproval)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.Approved && previousState == OrderState.OnTermination)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderbeingReapproved(orderId);
            }

            return null;
        }
    }
}
