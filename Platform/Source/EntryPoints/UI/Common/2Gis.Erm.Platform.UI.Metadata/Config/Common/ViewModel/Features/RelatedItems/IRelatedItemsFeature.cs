using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsFeature : IViewModelFeature
    {
        OldUIElementMetadata[] RelatedItems { get; }
    }
}
