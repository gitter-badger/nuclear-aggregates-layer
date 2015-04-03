using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsHost : IMetadataElementAspect
    {
        bool HasRelatedItems { get; }
        OldUIElementMetadata[] RelatedItems { get; }
    }
}
