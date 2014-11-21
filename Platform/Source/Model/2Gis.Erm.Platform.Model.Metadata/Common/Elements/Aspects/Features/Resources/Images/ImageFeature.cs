namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images
{
    public sealed class ImageFeature : IUniqueMetadataFeature
    {
        private readonly IImageDescriptor _imageDescriptor;

        public ImageFeature(IImageDescriptor imageDescriptor)
        {
            _imageDescriptor = imageDescriptor;
        }

        public IImageDescriptor ImageDescriptor
        {
            get { return _imageDescriptor; }
        }
    }
}
