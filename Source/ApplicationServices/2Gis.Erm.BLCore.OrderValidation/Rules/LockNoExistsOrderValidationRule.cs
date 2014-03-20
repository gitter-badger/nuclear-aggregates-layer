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
    /// Проверить на отсутсвие созданных блокировок по периоду
    /// </summary>
    public sealed class LockNoExistsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public LockNoExistsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
                    .Where(x => x.DestOrganizationUnitId == request.OrganizationUnitId)
                    .Where(o => o.Locks.Any(l => l.IsActive && !l.IsDeleted && l.PeriodStartDate == request.Period.Start && l.PeriodEndDate == request.Period.End))
                    .Select(o => new { o.Id, o.Number })
                    .ToList();

            if (orderDetails.Any())
            {
                foreach (var orderDetail in orderDetails)
                {
                    messages.Add(new OrderValidationMessage
                                     {
                                         Type = MessageType.Error,
                                         MessageText = string.Format(BLResources.OrdersCheckOrderHasLock, orderDetail.Number),
                                         OrderId = orderDetail.Id,
                                         OrderNumber = orderDetail.Number
                                     });
                }
            }
        }
    }
}
