using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public interface IActionsContained : IMetadataElementAspect
    {
        bool HasActions { get; }
        HierarchyMetadata[] ActionsDescriptors { get; }
    }
}