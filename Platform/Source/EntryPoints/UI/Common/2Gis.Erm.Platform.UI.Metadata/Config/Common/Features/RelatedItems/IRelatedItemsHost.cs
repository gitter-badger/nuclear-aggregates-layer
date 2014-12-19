using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public interface IRelatedItemsHost : IMetadataElementAspect
    {
        bool HasRelatedItems { get; }
        UIElementMetadata[] RelatedItems { get; }
    }
}
