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
    }
}