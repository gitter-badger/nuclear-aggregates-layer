using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes
{
    public sealed class ControlTypeFeature : ISecuredElementFeature
    {
        public ControlTypeFeature(IResourceDescriptor controlTypeDescriptor)
        {
            ControlTypeDescriptor = controlTypeDescriptor;
        }

        public IResourceDescriptor ControlTypeDescriptor { get; private set; }
    }
}