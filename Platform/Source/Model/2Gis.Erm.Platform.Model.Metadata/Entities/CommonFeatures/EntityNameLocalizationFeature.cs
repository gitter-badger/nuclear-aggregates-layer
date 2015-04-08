using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures
{
    public sealed class EntityNameLocalizationFeature : IUniqueMetadataFeature
    {
        public EntityNameLocalizationFeature(IStringResourceDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public IStringResourceDescriptor Descriptor { get; private set; }
    }
}
