using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.Actions
{
    public sealed class ActionsFeature : IViewModelFeature
    {
        private readonly UiElementMetadata[] _actionsDescriptors;

        public ActionsFeature(UiElementMetadata[] actionsDescriptors)
        {
            _actionsDescriptors = actionsDescriptors;
        }

        public UiElementMetadata[] ActionsDescriptors
        {
            get { return _actionsDescriptors; }
        }
    }
}
