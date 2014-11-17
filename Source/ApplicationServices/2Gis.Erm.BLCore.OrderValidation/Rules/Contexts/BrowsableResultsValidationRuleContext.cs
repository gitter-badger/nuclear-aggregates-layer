using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts
{
    public sealed class BrowsableResultsValidationRuleContext : IValidationRuleContext
    {
        private readonly HybridValidationParams _validationParams;
        private readonly Expression<Func<Order, bool>> _ordersFilterPredicate;
        private readonly IValidationResultsBrowser _validationResultsBrowser;

        public BrowsableResultsValidationRuleContext(
            HybridValidationParams validationParams,
            Expression<Func<Order, bool>> ordersFilterPredicate, 
            IValidationResultsBrowser validationResultsBrowser)
        {
            _validationParams = validationParams;
            _ordersFilterPredicate = ordersFilterPredicate;
            _validationResultsBrowser = validationResultsBrowser;
        }

        public Expression<Func<Order, bool>> OrdersFilterPredicate
        {
            get { return _ordersFilterPredicate; }
        }

        public IValidationResultsBrowser ResultsBrowser
        {
            get { return _validationResultsBrowser; }
        }

        public HybridValidationParams ValidationParams
        {
            get { return _validationParams; }
        }
    }
}