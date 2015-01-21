using NuClear.Metamodeling.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public sealed class ActionsFeature : IViewModelFeature
    {
        private readonly HierarchyMetadata[] _actionsDescriptors;

        public ActionsFeature(HierarchyMetadata[] actionsDescriptors)
        {
            _actionsDescriptors = actionsDescriptors;
        }

        public HierarchyMetadata[] ActionsDescriptors
        {
            get { return _actionsDescriptors; }
        }
    }
}
