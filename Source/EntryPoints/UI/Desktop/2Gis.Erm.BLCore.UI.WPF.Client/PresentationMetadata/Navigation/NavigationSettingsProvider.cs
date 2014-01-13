using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public sealed class NavigationSettingsProvider : INavigationSettingsProvider
    {
        private readonly IOperationsMetadataProvider _operationsMetadataProvider;
        private readonly HierarchyElement[] _settings;

        public NavigationSettingsProvider(IOperationsMetadataProvider operationsMetadataProvider)
        {
            _operationsMetadataProvider = operationsMetadataProvider;
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