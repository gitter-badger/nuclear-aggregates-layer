using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public sealed class RelatedItemsFeature : IRelatedItemsFeature
    {
        private readonly HierarchyElement[] _relatedItems;

        public RelatedItemsFeature(params HierarchyElement[] relatedItems)
        {
            _relatedItems = relatedItems;
        }

        public HierarchyElement[] RelatedItems
        {
            get
            {
                return _relatedItems;
            }
        }
    }
}