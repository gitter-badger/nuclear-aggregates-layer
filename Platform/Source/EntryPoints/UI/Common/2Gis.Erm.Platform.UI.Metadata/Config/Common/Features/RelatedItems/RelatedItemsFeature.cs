using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public sealed class RelatedItemsFeature : IRelatedItemsFeature
    {
        private readonly UIElementMetadata[] _relatedItems;

        public RelatedItemsFeature(params UIElementMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
        }

        public RelatedItemsFeature(IResourceDescriptor name, ITitleDescriptor title, params UIElementMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
            NameDescriptor = name;
            TitleDescriptor = title;
        }

        public IResourceDescriptor NameDescriptor { get; private set; }
        public ITitleDescriptor TitleDescriptor { get; private set; }

        public UIElementMetadata[] RelatedItems
        {
            get
            {
                return _relatedItems;
            }
        }
    }
}