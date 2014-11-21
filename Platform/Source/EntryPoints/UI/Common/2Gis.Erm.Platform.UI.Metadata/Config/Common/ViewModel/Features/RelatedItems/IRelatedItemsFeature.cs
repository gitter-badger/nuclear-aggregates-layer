using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        HierarchyMetadata[] RelatedItems { get; }
    }
}
