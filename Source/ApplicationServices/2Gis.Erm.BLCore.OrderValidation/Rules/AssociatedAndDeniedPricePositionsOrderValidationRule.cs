using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.OrderValidation.AssociatedAndDeniedPositions;
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

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            // ReSharper disable PossibleInvalidOperationException
            var checkRequest = request.Type == ValidationType.SingleOrderOnRegistration
                                  ? CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequest(request.OrderId.Value) // Для единичной проверки определен id заказа
                                  : CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForMassiveCheck(filterPredicate); // Для массовой проверки определено отделение организации
            // ReSharper restore PossibleInvalidOperationException
            var response = (ValidateOrdersResponse)_subRequestProcessor.HandleSubRequest(checkRequest, null);

            foreach (var message in response.Messages)
            {
                messages.Add(message);
            }
        }
    }
}
