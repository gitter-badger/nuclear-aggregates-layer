using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions
{
    public sealed class ActionsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IActionsContained, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IActionsContained
    {
        public ActionsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params UIElementMetadata[] actions)
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
