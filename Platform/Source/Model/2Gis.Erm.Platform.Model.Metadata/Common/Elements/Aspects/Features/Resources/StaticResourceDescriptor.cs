using System.Globalization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources
{
    public sealed class StaticStringResourceDescriptor : IStringResourceDescriptor
    {
        public StaticStringResourceDescriptor(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
        public string GetValue(CultureInfo culture)
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public string ResourceKeyToString()
        {
            return Value;
        }
    }
}