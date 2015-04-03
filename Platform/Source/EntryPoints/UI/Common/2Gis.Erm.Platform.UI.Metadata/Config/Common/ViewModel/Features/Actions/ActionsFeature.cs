using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Actions
{
    public sealed class ActionsFeature : IViewModelFeature
    {
        private readonly OldUIElementMetadata[] _actionsDescriptors;

        public ActionsFeature(OldUIElementMetadata[] actionsDescriptors)
        {
            _actionsDescriptors = actionsDescriptors;
        }

        public OldUIElementMetadata[] ActionsDescriptors
        {
            get { return _actionsDescriptors; }
        }
    }
}
