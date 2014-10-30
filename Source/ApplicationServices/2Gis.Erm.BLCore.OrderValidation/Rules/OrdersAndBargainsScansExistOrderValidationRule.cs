using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка на наличие сканов бланка заказа и договора
    /// </summary>
    public sealed class OrdersAndBargainsScansExistOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public OrdersAndBargainsScansExistOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orders = _finder.Find(filterPredicate)
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

            foreach (var order in orders)
            {
                if (!order.HasOrderScan)
                {
                    messages.Add(
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
                    messages.Add(new OrderValidationMessage
                                   {
                                       Type = MessageType.Warning, 
                                       OrderId = order.OrderId,
                                       OrderNumber = order.OrderNuber,
                                       MessageText = BLResources.OrderCheckOrderHasNoBargainScans
                                   });
                }
            }
        }
    }
}
