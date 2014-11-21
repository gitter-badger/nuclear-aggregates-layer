using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public interface IPropertyFeature : IMetadataFeature
    {
        EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
