using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Browser;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.DI;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.MainUI;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Presentation;
using DoubleGis.Erm.BLCore.UI.Web.Silverlight.UserProfiles.Services;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight
{
    public static class Bootstrapper
    {
        public static IUnityContainer Run(IAppSettings appSettings)
        {
            var container = ConfigureUnity(appSettings);
            InitializeUI(container);
            return container;
        }

        private static string GetWebAppEndpointUrl()
        {
            const string ErmEnvironmentTemplate = @"^/(?<Environment>Erm\d*)/";

            var applicationBaseUri = Application.Current.Host.Source;
            var environmentString = string.Empty;

            // проверим есть ли в url упоминание erm environment, если есть то его нужно будет учитывать
            var ermEnvironmentResolver = new Regex(ErmEnvironmentTemplate);
            var match = ermEnvironmentResolver.Match(applicationBaseUri.LocalPath);
            if (match.Success)
            {
                var group = match.Groups["Environment"];
                if (group.Success)
                {
                    environmentString = group.Value;
                }
            }

            return new UriBuilder
                {
                    Scheme = applicationBaseUri.Scheme,
                    Host = applicationBaseUri.Host,
                    Port = applicationBaseUri.Port,
                    Path = environmentString
                }.Uri.ToString();
        }

        private static IUnityContainer ConfigureUnity(IAppSettings appSettings)
        {
            var container = new UnityContainer()

                // настройка логирования, пока не используется
                .RegisterType<ILoggerFacade, EmptyLogger>(Lifetime.Singleton)

                // насторйка инфраструктуры Prism, пока не используется
                .RegisterType<IRegionManager, RegionManager>(Lifetime.Singleton)
                .RegisterType<IRegionViewRegistry, RegionViewRegistry>(Lifetime.Singleton)
                .RegisterType<IEventAggregator, EventAggregator>(Lifetime.Singleton)

                // настройка сервисов - поставщиков данных
                .RegisterType<IUserProfileDataProvider, UserProfileDataProvider>(Lifetime.Singleton,
                                                                                 new InjectionConstructor(GetWebAppEndpointUrl()))
                // .RegisterType<IUserProfileDataProvider, FakeProfileDataProvider>(Lifetime.Singleton)

                // настройка ViewModels
                .RegisterType<IUserLocaleInfoViewModel, UserLocaleInfoViewModel>(Lifetime.PerResolve,
                                                                                 new InjectionConstructor(
                                                                                     appSettings.UserCode,
                                                                                     typeof(IUserProfileDataProvider)))
                .RegisterType<IUserPersonalInfoViewModel, UserPersonalInfoViewModel>(Lifetime.PerResolve,
                                                                                     new InjectionConstructor(
                                                                                         appSettings.UserCode,
                                                                                         typeof(IUserProfileDataProvider)))
                .RegisterType<IUserProfilesMainViewModel, UserProfilesMainViewModel>(Lifetime.PerResolve,
                                                                                     new InjectionConstructor(
                                                                                         appSettings.UserCode,
                                                                                         typeof(IUserLocaleInfoViewModel),
                                                                                         typeof(IUserPersonalInfoViewModel),
                                                                                         typeof(IUserProfileDataProvider)));

            return container;
        }
        
        private static void InitializeUI(IUnityContainer container)
        {
            var viewModel = container.Resolve<IUserProfilesMainViewModel>();
            Application.Current.RootVisual = new Shell { DataContext = viewModel };
            HtmlPage.RegisterScriptableObject("SLMainViewModel", viewModel);
            HtmlPage.RegisterCreateableType("LookupField", typeof(LookupField));
            HtmlPage.RegisterCreateableType("EntityViewConfig", typeof(EntityViewConfig));
            HtmlPage.RegisterCreateableType("UserProfileViewModel", typeof(UserProfileViewModel));
        }
    }
}
