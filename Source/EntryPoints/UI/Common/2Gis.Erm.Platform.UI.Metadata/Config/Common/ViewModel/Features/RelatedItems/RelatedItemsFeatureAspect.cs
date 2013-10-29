using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public sealed class RelatedItemsFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IRelatedItemsHost, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IRelatedItemsHost
    {
        public RelatedItemsFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params HierarchyElement[] relatedItems)
        {
            if (relatedItems == null || relatedItems.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.Features.Add(new RelatedItemsFeature(relatedItems));
            return AspectHostBuilder;
        }
    }
}
