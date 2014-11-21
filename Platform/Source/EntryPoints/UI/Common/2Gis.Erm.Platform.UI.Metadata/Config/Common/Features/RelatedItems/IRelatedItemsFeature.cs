using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        HierarchyMetadata[] RelatedItems { get; }
    }
}
