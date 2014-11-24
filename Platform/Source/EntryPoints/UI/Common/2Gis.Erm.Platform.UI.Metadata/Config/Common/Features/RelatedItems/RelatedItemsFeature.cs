using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public sealed class RelatedItemsFeature : IRelatedItemsFeature
    {
        private readonly UiElementMetadata[] _relatedItems;

        public RelatedItemsFeature(params UiElementMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
        }

        public RelatedItemsFeature(IResourceDescriptor name, ITitleDescriptor title, params UiElementMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
            NameDescriptor = name;
            TitleDescriptor = title;
        }

        public IResourceDescriptor NameDescriptor { get; private set; }
        public ITitleDescriptor TitleDescriptor { get; private set; }

        public UiElementMetadata[] RelatedItems
        {
            get
            {
                return _relatedItems;
            }
        }
    }
}