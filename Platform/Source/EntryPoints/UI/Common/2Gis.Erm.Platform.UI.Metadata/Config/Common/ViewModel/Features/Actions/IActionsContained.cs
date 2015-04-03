using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public interface IActionsContained : IMetadataElementAspect
    {
        bool HasActions { get; }
        OldUIElementMetadata[] ActionsDescriptors { get; }
    }
}