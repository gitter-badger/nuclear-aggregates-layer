using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public sealed class RelatedItemsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IRelatedItemsHost, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IRelatedItemsHost
    {
        public RelatedItemsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params OldUIElementMetadata[] relatedItems)
        {
            if (relatedItems == null || relatedItems.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.WithFeatures(new RelatedItemsFeature(relatedItems));
            return AspectHostBuilder;
        }
    }
}
