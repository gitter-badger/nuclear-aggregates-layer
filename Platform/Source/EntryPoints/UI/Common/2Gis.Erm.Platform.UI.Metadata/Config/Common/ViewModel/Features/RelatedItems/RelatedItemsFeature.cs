using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public sealed class RelatedItemsFeature : IRelatedItemsFeature
    {
        private readonly OldUIElementMetadata[] _relatedItems;

        public RelatedItemsFeature(params OldUIElementMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
        }

        public OldUIElementMetadata[] RelatedItems
        {
            get
            {
                return _relatedItems;
            }
        }
    }
}