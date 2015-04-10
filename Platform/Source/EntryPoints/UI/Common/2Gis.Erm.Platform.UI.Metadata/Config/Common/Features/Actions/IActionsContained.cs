using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions
{
    public interface IActionsContained : IMetadataElementAspect
    {
        bool HasActions { get; }
        UIElementMetadata[] ActionsDescriptors { get; }
    }
}