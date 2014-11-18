using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts
{
    public sealed class MassOverridibleValidationRuleContext : IValidationRuleContext
    {
        private readonly MassOrdersValidationParams _validationParams;
        private readonly OrderValidationPredicate _combinedPredicate;

        public MassOverridibleValidationRuleContext(MassOrdersValidationParams validationParams, OrderValidationPredicate combinedPredicate)
        {
            _validationParams = validationParams;
            _combinedPredicate = combinedPredicate;
        }

        public MassOrdersValidationParams ValidationParams
        {
            get { return _validationParams; }
        }

        public OrderValidationPredicate CombinedPredicate
        {
            get { return _combinedPredicate; }
        }
    }
}