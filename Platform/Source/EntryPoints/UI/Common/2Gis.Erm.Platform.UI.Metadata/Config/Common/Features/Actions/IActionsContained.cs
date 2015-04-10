using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions
{
    public interface IActionsContained : IMetadataElementAspect
    {
        bool HasActions { get; }
        UIElementMetadata[] ActionsDescriptors { get; }
    }
}