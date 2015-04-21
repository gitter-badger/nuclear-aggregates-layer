using System;
using System.Globalization;

using NuClear.Security.API.UserContext.Profile;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo
{
    public sealed class NullUserInfo : IUserInfo
    {
        public static IUserInfo Default {
            get
            {
                var defaultLocaleInfo = LocaleInfo.Default;
                return new NullUserInfo("Test User")
                           {
                               Culture = defaultLocaleInfo.UserCultureInfo, 
                               Roles = new[] { "SystemAdministrator" }, 
                               TimeZone = defaultLocaleInfo.UserTimeZoneInfo
                           };
            }
        }

        private readonly string _fullName;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();

        public NullUserInfo(string fullName)
        {
            _fullName = fullName;
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
