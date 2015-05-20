using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

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
