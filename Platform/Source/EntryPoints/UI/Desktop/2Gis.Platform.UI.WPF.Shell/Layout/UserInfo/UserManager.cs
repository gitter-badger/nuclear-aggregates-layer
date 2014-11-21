using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.UserInfo
{
    public sealed class UserManager : IUserManagerViewModel
    {
        public UserManager(ILayoutComponentsRegistry componentsRegistry, IUserInfo userInfo)
        {
            Info = userInfo;
            ViewSelector = 
                new ComponentSelector<ILayoutUserInfoComponent, IUserInfo>(componentsRegistry.GetComponentsForLayoutRegion<ILayoutUserInfoComponent>());
        }

        public IUserInfo Info { get; private set; }
        public DataTemplateSelector ViewSelector { get; private set; }
    }
}
