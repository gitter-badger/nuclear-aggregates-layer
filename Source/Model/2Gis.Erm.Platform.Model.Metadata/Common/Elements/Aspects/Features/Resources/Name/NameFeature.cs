namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name
{
    public sealed class NameFeature : IUniqueMetadataFeature
    {
        public NameFeature(IResourceDescriptor nameDescriptor)
        {
            NameDescriptor = nameDescriptor;
        }

        public IResourceDescriptor NameDescriptor { get; private set; }
    }
}
