using System.Resources;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class EnumPropertyFeature : IPropertyFeature
    {
        public EnumPropertyFeature(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }

        public ResourceManager ResourceManager { get; private set; }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
