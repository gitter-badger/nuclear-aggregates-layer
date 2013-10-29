namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images
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