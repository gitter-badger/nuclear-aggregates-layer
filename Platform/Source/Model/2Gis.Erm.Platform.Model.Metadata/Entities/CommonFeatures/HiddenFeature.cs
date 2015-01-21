using NuClear.Metamodeling.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures
{
    public sealed class HiddenFeature : IPropertyFeature, IDataFieldFeature
    {
        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
