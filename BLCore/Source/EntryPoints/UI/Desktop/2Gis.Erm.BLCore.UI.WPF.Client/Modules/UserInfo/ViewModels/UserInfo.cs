using System;
using System.Globalization;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.UserInfo.ViewModels
{
    public sealed class UserInfo : IUserInfo
    {
        private readonly IShellSettings _shellSettings;

        private readonly string _fullName;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        public UserInfo(IShellSettings shellSettings)
        {
            _shellSettings = shellSettings;

            var defaultInfo = NullUserInfo.Default;
            _fullName = defaultInfo.FullName;
            Roles = defaultInfo.Roles;
            TimeZone = defaultInfo.TimeZone;
            Culture = _shellSettings.TargetCulture;
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        public string[] Roles { get; set; }
        public CultureInfo Culture { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
    }
}