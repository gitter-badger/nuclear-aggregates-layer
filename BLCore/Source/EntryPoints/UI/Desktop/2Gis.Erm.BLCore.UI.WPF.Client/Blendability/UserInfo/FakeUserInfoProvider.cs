using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.UserInfo
{
    public static class FakeUserInfoProvider
    {
        public static IUnityContainer RegisterUserInfo(this IUnityContainer container)
        {
            return container.RegisterInstance(typeof(IUserInfo), NullUserInfo.Default, Lifetime.Singleton);
        }
    }
}
