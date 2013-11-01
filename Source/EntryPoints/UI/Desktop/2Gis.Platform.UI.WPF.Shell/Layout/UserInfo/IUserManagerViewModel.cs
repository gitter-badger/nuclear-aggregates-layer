using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.UserInfo
{
    public interface IUserManagerViewModel
    {
        IUserInfo Info { get; }
        DataTemplateSelector ViewSelector { get; }
    }
}
