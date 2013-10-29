using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        HierarchyElement[] RelatedItems { get; }
    }
}
