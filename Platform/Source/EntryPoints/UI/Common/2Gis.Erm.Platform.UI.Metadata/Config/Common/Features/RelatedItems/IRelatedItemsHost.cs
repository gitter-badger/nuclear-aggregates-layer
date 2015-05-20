using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public interface IRelatedItemsHost : IMetadataElementAspect
    {
        bool HasRelatedItems { get; }
        UIElementMetadata[] RelatedItems { get; }
    }
}
