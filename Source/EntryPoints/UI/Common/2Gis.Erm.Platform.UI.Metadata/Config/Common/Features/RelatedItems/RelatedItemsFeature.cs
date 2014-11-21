using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public sealed class RelatedItemsFeature : IRelatedItemsFeature
    {
        private readonly HierarchyMetadata[] _relatedItems;

        public RelatedItemsFeature(params HierarchyMetadata[] relatedItems)
        {
            _relatedItems = relatedItems;
        }

        public HierarchyMetadata[] RelatedItems
        {
            get
            {
                return _relatedItems;
            }
        }
    }
}