using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить, что в заказе есть хотя бы одна позиция
    /// </summary>
    public sealed class OrderHasAtLeastOnePositionOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public OrderHasAtLeastOnePositionOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _finder.Find(ruleContext.OrdersFilterPredicate)
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