using System;
using System.Globalization;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo
{
    public interface IUserInfo : ILayoutComponentViewModel
    {
        string FullName { get; }
        string[] Roles { get; }
        CultureInfo Culture { get; }
        TimeZoneInfo TimeZone { get; }
    }
}
