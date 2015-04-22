using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications.Views;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Notifications
{
    public class NotificationsModule : IModule, IDesignTimeModule
    {
        private readonly IUnityContainer _container;

        public NotificationsModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get { return new Guid("4FE2AC4F-D766-4168-B886-3F778A94D2F3"); }
        }

        public string Description
        {
            get { return "Notifications and errors viewing module"; }
        }

        public void Configure()
        {
            _container
                .RegisterType<INotificationList, NotificationsListViewModel>(Lifetime.Singleton)
                .RegisterType<INotificationsManager, NotificationsListViewModel>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutNotificationsComponent, NotificationsComponent<NotificationsListViewModel, NotificationsListControl>>(Lifetime.Singleton);
        }

        #region Design time

        void IDesignTimeModule.Configure()
        {
            _container
                .RegisterType<INotificationList, NotificationsListViewModel>(Lifetime.Singleton)
                .RegisterType<INotificationsManager, NotificationsListViewModel>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutNotificationsComponent, NotificationsComponent<NotificationsListViewModel, NotificationsListControl>>(Lifetime.Singleton);
        }

        #endregion
    }
}