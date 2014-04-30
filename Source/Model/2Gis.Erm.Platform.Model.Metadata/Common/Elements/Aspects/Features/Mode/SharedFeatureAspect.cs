namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Mode
{
    public sealed class SharedFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, ISharable, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, ISharable
    {
        public SharedFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Shared()
        {
            AspectHostBuilder.WithFeatures(new SharedFeature());
            return AspectHostBuilder;
        }
    }
}
