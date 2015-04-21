using System.Globalization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images
{
    public sealed class StaticPathImageDescriptor : IImageDescriptor
    {
        private readonly string _iconPath;

        public StaticPathImageDescriptor(string iconPath)
        {
            _iconPath = iconPath;
        }

        public string IconPath
        {
            get { return _iconPath; }
        }

        public object GetValue(CultureInfo culture)
        {
            return IconPath;
        }

        public string ResourceKeyToString()
        {
            return IconPath;
        }

        public override string ToString()
        {
            return IconPath;
        }
    }
}