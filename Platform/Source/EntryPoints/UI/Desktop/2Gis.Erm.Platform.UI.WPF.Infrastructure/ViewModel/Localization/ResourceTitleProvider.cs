using System;
using System.Resources;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.ResourceUtilities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public sealed class ResourceTitleProvider : ITitleProvider
    {
        private readonly ResourceTitleDescriptor _descriptor;
        private readonly IUserInfo _userInfo;
        private readonly Lazy<ResourceManager> _resourceManager;

        public ResourceTitleProvider(ResourceTitleDescriptor descriptor, IUserInfo userInfo)
        {
            _descriptor = descriptor;
            _userInfo = userInfo;
            _resourceManager = new Lazy<ResourceManager>(GetResourceManager);
        }

        public string Title
        {
            get
            {
                return GetTitle();
            }
        }

        private string GetTitle()
        {
            var defaultValue = "NotFound_" + _descriptor.ResourceEntryKey.ResourceEntryName;
            var resourceManager = _resourceManager.Value;
            if (resourceManager == null)
            {
                return defaultValue;
            }

            var entryValue = resourceManager.GetString(_descriptor.ResourceEntryKey.ResourceEntryName, _userInfo.Culture);
            return !string.IsNullOrEmpty(entryValue) ? entryValue : defaultValue;
        }

        private ResourceManager GetResourceManager()
        {
            ResourceManager resourceManager;
            return _descriptor.ResourceEntryKey.ResourceHostType.TryResolveResourceManager(out resourceManager) ? resourceManager : null;
        }
    }
}
