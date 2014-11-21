using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts
{
    public sealed class OrdinaryValidationRuleContext : IValidationRuleContext
    {
        private readonly bool _isMassValidation;
        private readonly Expression<Func<Order, bool>> _ordersFilterPredicate;

        public OrdinaryValidationRuleContext(bool isMassValidation, Expression<Func<Order, bool>> ordersFilterPredicate)
        {
            _isMassValidation = isMassValidation;
            _ordersFilterPredicate = ordersFilterPredicate;
        }

        public Expression<Func<Order, bool>> OrdersFilterPredicate
        {
            get { return _ordersFilterPredicate; }
        }

        public bool IsMassValidation
        {
            get { return _isMassValidation; }
        }
    }
}