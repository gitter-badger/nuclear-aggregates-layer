using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions
{
    public sealed class ActionsFeature : IViewModelFeature
    {
        private readonly UIElementMetadata[] _actionsDescriptors;

        public ActionsFeature(UIElementMetadata[] actionsDescriptors)
        {
            _actionsDescriptors = actionsDescriptors;
        }

        public UIElementMetadata[] ActionsDescriptors
        {
            get { return _actionsDescriptors; }
        }
    }
}
