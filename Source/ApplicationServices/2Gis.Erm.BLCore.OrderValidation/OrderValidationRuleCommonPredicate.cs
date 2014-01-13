using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public abstract class OrderValidationRuleCommonPredicate : OrderValidationRuleBase
    {
        protected override void Validate(ValidateOrdersRequest request, OrderValidationPredicate filterPredicate, IList<OrderValidationMessage> messages)
        {
            ValidateInternal(request, filterPredicate.GetCombinedPredicate(), messages);
        }

        protected abstract void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages);
    }
}