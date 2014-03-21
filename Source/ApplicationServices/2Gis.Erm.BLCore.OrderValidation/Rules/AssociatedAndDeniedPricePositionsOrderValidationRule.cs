using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.OrderValidation.AssociatedAndDeniedPositions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить являются ли позиции заказа сопутствующими к позициям которых нет в выборке по активным заказам 
    /// И
    /// Проверить являются ли позиции заказа запрещёнными к какой либо из позиций присутствующей в выборке по активным заказам
    /// </summary>
    public sealed class AssociatedAndDeniedPricePositionsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly ISubRequestProcessor _subRequestProcessor;

        public AssociatedAndDeniedPricePositionsOrderValidationRule(ISubRequestProcessor subRequestProcessor)
        {
            _subRequestProcessor = subRequestProcessor;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request,
                                                 Expression<Func<Order, bool>> filterPredicate,
                                                 IEnumerable<long> invalidOrderIds,
                                                 IList<OrderValidationMessage> messages)
        {
            CheckOrdersAssociatedAndDeniedPositionsRequest checkRequest;
            switch (request.Type)
            {
                // Для единичной проверки определен id заказа
                case ValidationType.SingleOrderOnRegistration:
                    checkRequest = CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequest(request.OrderId.Value);
                    break;

                // Для единичной проверки определен id заказа
                case ValidationType.SingleOrderOnStateChanging:
                    checkRequest = CreateSingleCheckRequest(request.OrderId.Value, request.CurrentOrderState, request.NewOrderState);
                    break;

                // Для массовой проверки определено отделение организации
                default:
                    checkRequest = CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForMassiveCheck(filterPredicate);
                    break;
            }

            if (checkRequest == null)
            {
                return;
            }

            var checkResponse = (ValidateOrdersResponse)_subRequestProcessor.HandleSubRequest(checkRequest, null);

            foreach (var message in checkResponse.Messages)
            {
                messages.Add(message);
            }
        }

        private static CheckOrdersAssociatedAndDeniedPositionsRequest CreateSingleCheckRequest(long orderId, OrderState previousState, OrderState newState)
        {
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
