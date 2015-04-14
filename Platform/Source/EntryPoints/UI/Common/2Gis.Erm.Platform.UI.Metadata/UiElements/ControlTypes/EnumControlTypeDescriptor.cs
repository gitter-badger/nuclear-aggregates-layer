using System.Globalization;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes
{
    public sealed class EnumControlTypeDescriptor : IResourceDescriptor
    {
        public EnumControlTypeDescriptor(ControlType type)
        {
            Type = type;
        }

        public ControlType Type { get; private set; }
        public object GetValue(CultureInfo culture)
        {
            return Type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public string ResourceKeyToString()
        {
            return Type.ToString();
        }
    }
}