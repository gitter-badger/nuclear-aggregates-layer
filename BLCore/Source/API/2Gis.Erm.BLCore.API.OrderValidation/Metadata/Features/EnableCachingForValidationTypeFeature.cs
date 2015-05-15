using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features
{
    public sealed class EnableCachingForValidationTypeFeature : IMetadataFeature
    {
        private readonly ValidationType _validationType;

        public EnableCachingForValidationTypeFeature(ValidationType validationType)
        {
            _validationType = validationType;
        }

        public ValidationType ValidationType
        {
            get { return _validationType; }
        }
    }
}