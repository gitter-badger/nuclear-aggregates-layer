using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo
{
    public sealed class UserInfoComponent<TViewModel, TView>
        : InstanceIndependentLayoutComponent<ILayoutUserInfoComponent, IUserInfo, TViewModel, TView>, ILayoutUserInfoComponent
        where TView : Control
        where TViewModel : class, IUserInfo
    {
    }
}
