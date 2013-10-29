namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images
{
    public sealed class ImageFeature : IConfigFeature
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
