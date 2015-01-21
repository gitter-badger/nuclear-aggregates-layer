using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.RelatedItems
{
    public interface IRelatedItemsHost : IMetadataElementAspect
    {
        bool HasRelatedItems { get; }
        HierarchyMetadata[] RelatedItems { get; }
    }
}
