namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name
{
    public sealed class NameFeature : IUniqueMetadataFeature
    {
        public NameFeature(IStringResourceDescriptor nameDescriptor)
        {
            NameDescriptor = nameDescriptor;
        }

        public IStringResourceDescriptor NameDescriptor { get; private set; }
    }
}