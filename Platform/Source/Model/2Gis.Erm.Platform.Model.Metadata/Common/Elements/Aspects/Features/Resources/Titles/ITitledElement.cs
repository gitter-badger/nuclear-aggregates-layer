namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles
{
    public interface ITitledElement : IMetadataElementAspect
    {
        ITitleDescriptor TitleDescriptor { get; }
    }
}
