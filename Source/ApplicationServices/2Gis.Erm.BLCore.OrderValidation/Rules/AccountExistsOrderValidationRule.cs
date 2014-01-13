using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на наличие привязки к лицевому счёту
    /// </summary>
    public sealed class AccountExistsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public AccountExistsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var orderDetails = _finder.Find(filterPredicate)
                    .Where(o => o.AccountId == null || (!o.Account.IsActive && o.Account.IsDeleted))
                    .Select(o => new { o.Id, o.Number })
                    .ToList();

            foreach (var orderDetail in orderDetails)
            {
                string orderDescription = GenerateDescription(EntityName.Order, orderDetail.Number, orderDetail.Id);
                messages.Add(new OrderValidationMessage
                {
                    Type = MessageType.Error,
                    MessageText = string.Format(BLResources.OrdersCheckOrderHasNoAccount, orderDescription),
                    OrderId = orderDetail.Id,
                    OrderNumber = orderDetail.Number
                });
            }
        }
    }
}
