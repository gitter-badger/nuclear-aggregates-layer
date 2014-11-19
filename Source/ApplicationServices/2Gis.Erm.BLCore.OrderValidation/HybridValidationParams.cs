using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class HybridValidationParams
    {
        private readonly MassOrdersValidationParams _massOrdersValidationParams;
        private readonly SingleOrderValidationParams _singleOrderValidationParams;
        private readonly bool _isMassValidation;

        public HybridValidationParams(ValidationParams validationParams)
        {
            var massOrdersValidationParams = validationParams as MassOrdersValidationParams;
            if (massOrdersValidationParams != null)
            {
                _massOrdersValidationParams = massOrdersValidationParams;
                _isMassValidation = true;
            }
            else
            {
                _singleOrderValidationParams = (SingleOrderValidationParams)validationParams; 
            }
        }

        public MassOrdersValidationParams Mass
        {
            get { return _massOrdersValidationParams; }
        }

        public SingleOrderValidationParams Single
        {
            get { return _singleOrderValidationParams; }
        }

        public bool IsMassValidation
        {
            get { return _isMassValidation; }
        }

        public TimePeriod Period
        {
            get { return IsMassValidation ? _massOrdersValidationParams.Period : _singleOrderValidationParams.Period; }
        }

        public ValidationType Type
        {
            get { return IsMassValidation ? _massOrdersValidationParams.Type : _singleOrderValidationParams.Type; }
        }
    }
}