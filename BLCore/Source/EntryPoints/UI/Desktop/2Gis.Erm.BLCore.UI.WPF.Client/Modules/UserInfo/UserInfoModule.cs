using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.UserInfo;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.UserInfo.Views;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Blendability.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.UserInfo
{
    public sealed class UserInfoModule : IModule, IDesignTimeModule
    {
        private readonly IUnityContainer _container;

        public UserInfoModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get
            {
                return new Guid("2F7F1175-579C-4F22-97AA-660AB8CFF7AD");
            }
        }
        public string Description
        {
            get
            {
                return "User information module";
            }
        }

        public void Configure()
        {
            _container
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutUserInfoComponent, UserInfoComponent<ViewModels.UserInfo, UserInfoControl>>(Lifetime.Singleton)
                .RegisterType<IUserInfo, ViewModels.UserInfo>(Lifetime.Singleton);
        }

        #region Design time

        void IDesignTimeModule.Configure()
        {
            _container
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutUserInfoComponent, UserInfoComponent<NullUserInfo, UserInfoControl>>(Lifetime.Singleton)
                .RegisterUserInfo();
        }

        #endregion
    }
}
