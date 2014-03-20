using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить, что в заказе есть хотя бы одна позиция
    /// </summary>
    public sealed class OrderHasAtLeastOnePositionOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public OrderHasAtLeastOnePositionOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
                .Where(order => !order.OrderPositions.Any(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted))
                .Select(order => new { order.Id, order.Number })
                .ToArray();

            foreach (var orderDetail in orderDetails)
            {
                messages.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        MessageText = BLResources.OrderCheckOrderHasNoPositions,
                        OrderId = orderDetail.Id,
                        OrderNumber = orderDetail.Number
                    });
            }
        }
    }
}