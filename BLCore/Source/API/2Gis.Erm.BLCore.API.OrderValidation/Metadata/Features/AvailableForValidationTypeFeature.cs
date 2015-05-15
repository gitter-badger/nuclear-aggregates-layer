using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features
{
    public sealed class AvailableForValidationTypeFeature : IMetadataFeature
    {
        private readonly ValidationType _validationType;

        public AvailableForValidationTypeFeature(ValidationType validationType)
        {
            _validationType = validationType;
        }

        public ValidationType ValidationType
        {
            get { return _validationType; }
        }
    }
}