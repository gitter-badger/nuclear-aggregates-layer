using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка заказа на наличие сформированных счетов на оплату
    /// </summary>
    public sealed class BillsSumsOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public BillsSumsOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var orderDetails = _query.For<Order>()
                .Where(ruleContext.OrdersFilterPredicate)
                .Select(order => new
                                     {
                                         OrderId = order.Id,
                                         OrderNumber = order.Number,
                                         order.BeginDistributionDate,
                                         order.EndDistributionDatePlan,
                                         BillsSum = order.Bills.Where(bill => bill.IsActive && !bill.IsDeleted).Sum(bill => (decimal?)bill.PayablePlan),
                                         BillBeginDistributionDate = order.Bills.Where(bill => bill.IsActive && !bill.IsDeleted).Min<Bill, DateTime?>(bill => bill.BeginDistributionDate),
                                         BillEndDistributionDate = order.Bills.Where(bill => bill.IsActive && !bill.IsDeleted).Max<Bill, DateTime?>(bill => bill.EndDistributionDate),
                                         OrderSum = order.PayablePlan,

                                         // При заказах на нулевую сумму не требуется создавать счета. Но если они есть - надо проверить.
                                         // http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=95749688
                                         OrderCheckCanBeSkipped = order.PayablePlan == 0 && order.Bills.Count(bill => !bill.IsDeleted) == 0
                                     })
                .Where(order => order.BillsSum == null || order.BillsSum != order.OrderSum
                    || (order.BillBeginDistributionDate.HasValue && order.BeginDistributionDate != order.BillBeginDistributionDate)
                    || (order.BillEndDistributionDate.HasValue && order.EndDistributionDatePlan != order.BillEndDistributionDate))
                .Where(order => !order.OrderCheckCanBeSkipped)
                .ToArray();

            var results = new List<OrderValidationMessage>();

            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.BillsSum == null || orderDetail.BillsSum != orderDetail.OrderSum)
                {
                    results.Add(new OrderValidationMessage
                                      {
                                          Type = MessageType.Error,
                                          OrderId = orderDetail.OrderId,
                                          OrderNumber = orderDetail.OrderNumber,
                                          MessageText = orderDetail.BillsSum == null
                                                            ? BLResources.OrdersCheckNeedToCreateBills
                                                            : BLResources.OrderApproval_BillsSumDoesntMatchWhenProcessingOrderOnApproval
                                      });
                }

                if ((orderDetail.BillBeginDistributionDate.HasValue && orderDetail.BeginDistributionDate != orderDetail.BillBeginDistributionDate)
                    || (orderDetail.BillEndDistributionDate.HasValue && orderDetail.EndDistributionDatePlan != orderDetail.BillEndDistributionDate))
                {
                    results.Add(new OrderValidationMessage
                                   {
                                       Type = MessageType.Error,
                                       OrderId = orderDetail.OrderId,
                                       OrderNumber = orderDetail.OrderNumber,
                                       MessageText = BLResources.OrderCheckOrderAndBillHaveDifferentPeriods
                                   });
                }
            }

            return results;
        }
    }
}
