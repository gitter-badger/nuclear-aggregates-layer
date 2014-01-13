using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public sealed class FakeNavigationSettingsProvider : INavigationSettingsProvider
    {
        private readonly HierarchyElement[] _settings;
        public FakeNavigationSettingsProvider()
        {
            _settings = NavigationSettings.Settings;
        }

        public HierarchyElement[] Settings
        {
            get
            {
                return _settings;
            }
        }
    }
}