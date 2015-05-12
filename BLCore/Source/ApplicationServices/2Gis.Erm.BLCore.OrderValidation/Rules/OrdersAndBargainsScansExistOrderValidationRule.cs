using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка на наличие сканов бланка заказа и договора
    /// </summary>
    public sealed class OrdersAndBargainsScansExistOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public OrdersAndBargainsScansExistOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var orders = _finder.Find(ruleContext.OrdersFilterPredicate)
                .Select(order => new
                                     {
                                         OrderId = order.Id,
                                         OrderNuber = order.Number,
                                         HasOrderScan = order.OrderFiles.Any(y => y.IsActive && !y.IsDeleted && y.FileKind == OrderFileKind.OrderScan),
                                         HasBargain = order.Bargain != null,
                                         HasBargainScan = order.Bargain != null && order.Bargain.BargainFiles.Any(y => y.IsActive && !y.IsDeleted && y.FileKind == BargainFileKind.BargainScan)
                                     })
                .Where(order => !order.HasOrderScan || (order.HasBargain && !order.HasBargainScan))
                .ToArray();

            var results = new List<OrderValidationMessage>();

            foreach (var order in orders)
            {
                if (!order.HasOrderScan)
                {
                    results.Add(
                        new OrderValidationMessage
                            {
                                Type = MessageType.Warning,
                                OrderId = order.OrderId,
                                OrderNumber = order.OrderNuber,
                                MessageText = BLResources.OrderCheckOrderHasNoScans
                            });
                }

                if (order.HasBargain && !order.HasBargainScan)
                {
                    results.Add(new OrderValidationMessage
                                   {
                                       Type = MessageType.Warning, 
                                       OrderId = order.OrderId,
                                       OrderNumber = order.OrderNuber,
                                       MessageText = BLResources.OrderCheckOrderHasNoBargainScans
                                   });
                }
            }

            return results;
        }
    }
}
