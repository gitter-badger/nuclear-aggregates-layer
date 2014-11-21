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
    }
}