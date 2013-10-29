using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public sealed class ActionsFeature : IViewModelFeature
    {
        private readonly HierarchyElement[] _actionsDescriptors;

        public ActionsFeature(HierarchyElement[] actionsDescriptors)
        {
            _actionsDescriptors = actionsDescriptors;
        }

        public HierarchyElement[] ActionsDescriptors
        {
            get { return _actionsDescriptors; }
        }
    }
}
