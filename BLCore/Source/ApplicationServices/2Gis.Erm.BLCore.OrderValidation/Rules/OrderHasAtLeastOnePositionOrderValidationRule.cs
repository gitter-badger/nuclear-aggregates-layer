using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить, что в заказе есть хотя бы одна позиция
    /// </summary>
    public sealed class OrderHasAtLeastOnePositionOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public OrderHasAtLeastOnePositionOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _query.For<Order>()
                          .Where(ruleContext.OrdersFilterPredicate)
                          .Where(order => !order.OrderPositions.Any(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted))
                          .Select(order => new { order.Id, order.Number })
                          .AsEnumerable()
                          .Select(x => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               MessageText = BLResources.OrderCheckOrderHasNoPositions,
                                               OrderId = x.Id,
                                               OrderNumber = x.Number
                                           });
        }
    }
}