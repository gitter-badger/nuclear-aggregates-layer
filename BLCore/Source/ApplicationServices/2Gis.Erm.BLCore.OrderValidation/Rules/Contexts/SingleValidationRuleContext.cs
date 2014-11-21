using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts
{
    public sealed class SingleValidationRuleContext : IValidationRuleContext
    {
        private readonly SingleOrderValidationParams _validationParams;

        public SingleValidationRuleContext(SingleOrderValidationParams validationParams)
        {
            _validationParams = validationParams;
        }

        public SingleOrderValidationParams ValidationParams
        {
            get { return _validationParams; }
        }
    }
}