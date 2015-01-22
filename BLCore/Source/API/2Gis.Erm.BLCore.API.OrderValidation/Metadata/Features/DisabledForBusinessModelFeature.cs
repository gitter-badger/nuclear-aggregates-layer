using DoubleGis.Erm.Platform.Model;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features
{
    public sealed class DisabledForBusinessModelFeature : IMetadataFeature
    {
        private readonly BusinessModel _businessModel;

        public DisabledForBusinessModelFeature(BusinessModel businessModel)
        {
            _businessModel = businessModel;
        }

        public BusinessModel BusinessModel
        {
            get { return _businessModel; }
        }
    }
}