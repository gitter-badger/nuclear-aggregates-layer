namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles
{
    public sealed class TitleFeature : IUniqueMetadataFeature
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
