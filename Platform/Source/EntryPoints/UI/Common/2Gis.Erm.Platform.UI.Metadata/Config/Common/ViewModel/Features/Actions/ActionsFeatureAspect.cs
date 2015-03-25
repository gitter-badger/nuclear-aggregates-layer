using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public sealed class ActionsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IActionsContained, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IActionsContained
    {
        public ActionsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params OldUIElementMetadata[] actions)
        {
            if (actions == null || actions.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.WithFeatures(new ActionsFeature(actions));
            return AspectHostBuilder;
        }
    }
}
