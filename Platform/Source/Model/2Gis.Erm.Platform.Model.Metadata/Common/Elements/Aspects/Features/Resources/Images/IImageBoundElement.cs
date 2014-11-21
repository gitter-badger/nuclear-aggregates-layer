namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images
{
    public interface IImageBoundElement : IMetadataElementAspect
    {
        IImageDescriptor ImageDescriptor { get; }
    }
}