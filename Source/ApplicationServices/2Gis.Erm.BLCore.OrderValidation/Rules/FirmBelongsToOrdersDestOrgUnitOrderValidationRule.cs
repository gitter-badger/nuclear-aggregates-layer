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
    /// Проверить соответствие города назначения отделению организации за которым закреплена фирма, выбранная в заказ
    /// </summary>
    public sealed class FirmBelongsToOrdersDestOrgUnitOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public FirmBelongsToOrdersDestOrgUnitOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
                    .Where(order => order.Firm.OrganizationUnitId != order.DestOrganizationUnitId)
                    .Select(order => new { order.Id, order.Number, order.FirmId })
                    .ToList();

            if (orderDetails.Count > 0)
            {
                foreach (var orderDetail in orderDetails)
                {
                    messages.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = string.Format(BLResources.OrdersCheckDestOrganizationUnitDoesntMatchFirmsOne, orderDetail.Number),
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }
            }
        }
    }
}