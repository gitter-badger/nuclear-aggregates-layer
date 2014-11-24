using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes
{
    public sealed class EnumControlTypeDescriptor : IResourceDescriptor
    {
        public EnumControlTypeDescriptor(ControlType type)
        {
            Type = type;
        }

        public ControlType Type { get; private set; }
    }
}