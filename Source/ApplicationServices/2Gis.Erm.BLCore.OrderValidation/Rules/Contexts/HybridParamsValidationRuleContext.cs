using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts
{
    public sealed class HybridParamsValidationRuleContext : IValidationRuleContext
    {
        private readonly HybridValidationParams _validationParams;
        private readonly Expression<Func<Order, bool>> _ordersFilterPredicate;

        public HybridParamsValidationRuleContext(HybridValidationParams validationParams, Expression<Func<Order, bool>> ordersFilterPredicate)
        {
            _validationParams = validationParams;
            _ordersFilterPredicate = ordersFilterPredicate;
        }

        public HybridValidationParams ValidationParams
        {
            get { return _validationParams; }
        }

        public Expression<Func<Order, bool>> OrdersFilterPredicate
        {
            get { return _ordersFilterPredicate; }
        }
    }
}