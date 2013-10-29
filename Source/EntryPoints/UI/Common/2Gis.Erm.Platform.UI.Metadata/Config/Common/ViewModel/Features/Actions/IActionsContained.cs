using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public interface IActionsContained : IConfigElementAspect
    {
        bool HasActions { get; }
        HierarchyElement[] ActionsDescriptors { get; }
    }
}