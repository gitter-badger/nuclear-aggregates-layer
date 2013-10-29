namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles
{
    public sealed class TitleFeature : IConfigFeature
    {
        private readonly ITitleDescriptor _titleDescriptor;

        public TitleFeature(ITitleDescriptor titleDescriptor)
        {
            _titleDescriptor = titleDescriptor;
        }

        public ITitleDescriptor TitleDescriptor
        {
            get { return _titleDescriptor; }
        }
    }
}
