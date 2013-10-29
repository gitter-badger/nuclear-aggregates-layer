namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images
{
    public interface IImageBoundElement : IConfigElementAspect
    {
        IImageDescriptor ImageDescriptor { get; }
    }
}