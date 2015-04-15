namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name
{
    public interface INamedElement : IMetadataElementAspect
    {
        IStringResourceDescriptor NameDescriptor { get; }
    }
}
