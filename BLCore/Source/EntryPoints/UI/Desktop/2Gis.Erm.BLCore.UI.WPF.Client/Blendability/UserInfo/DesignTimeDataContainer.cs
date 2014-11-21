using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability
// ReSharper restore CheckNamespace
{
    public static partial class DesignTimeDataContainer
    {
        public static class UserInfo
        {
            public static IUserInfo DefualtUserInfo
            {
                get
                {
                    return NullUserInfo.Default;
                }
            }
        }
    }
}
