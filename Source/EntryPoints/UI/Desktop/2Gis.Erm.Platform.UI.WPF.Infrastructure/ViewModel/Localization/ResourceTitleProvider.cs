using System;
using System.Reflection;
using System.Resources;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public sealed class ResourceTitleProvider : ITitleProvider
    {
        private readonly ResourceTitleDescriptor _descriptor;
        private readonly IUserInfo _userInfo;

        public ResourceTitleProvider(ResourceTitleDescriptor descriptor, IUserInfo userInfo)
        {
            _descriptor = descriptor;
            _userInfo = userInfo;
            _resourceManager = new Lazy<ResourceManager>(GetResourceManger);
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

        private readonly Lazy<ResourceManager> _resourceManager;

        private ResourceManager GetResourceManger()
        {
            const string ResoureManagerPropertyName = "ResourceManager";
            var resourceManagerProperty = _descriptor.ResourceEntryKey.ResourceHostType.GetProperty(ResoureManagerPropertyName, BindingFlags.Public | BindingFlags.Static);
            if (resourceManagerProperty == null)
            {
                throw new InvalidOperationException("Can't find required property " + ResoureManagerPropertyName + " in type " + _descriptor.ResourceEntryKey.ResourceHostType);
            }

            return (ResourceManager)resourceManagerProperty.GetValue(null);
        }
    }
}
