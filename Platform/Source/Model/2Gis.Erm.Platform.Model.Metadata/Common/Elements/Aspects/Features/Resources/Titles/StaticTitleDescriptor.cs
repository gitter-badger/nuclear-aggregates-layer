using System.Globalization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles
{
    public sealed class StaticTitleDescriptor : ITitleDescriptor
    {
        private readonly string _title;

        public StaticTitleDescriptor(string title)
        {
            _title = title;
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string GetValue(CultureInfo culture)
        {
            return Title;
        }

        public string ResourceKeyToString()
        {
            return Title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}