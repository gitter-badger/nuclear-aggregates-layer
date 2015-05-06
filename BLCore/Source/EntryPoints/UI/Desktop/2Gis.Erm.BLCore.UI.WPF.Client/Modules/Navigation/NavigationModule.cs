using System;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.Views;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation
{
    public sealed class NavigationModule : IModule, IDesignTimeStandaloneWorkerModule
    {
        private readonly IUnityContainer _container;
        private Timer _timer;

        public NavigationModule(IUnityContainer container)
        {
            _container = container;
           
        }

        public Guid Id
        {
            get
            {
                return new Guid("5C35090F-48FD-4511-BA69-9BC11CC5DC03");
            }
        }

        public string Description
        {
            get
            {
                return "Erm WPF Client Navigation region infrastructure";
            }
        }

        public void Configure()
        {
            AddNavigationComponents();
            _container.RegisterType<INavigationAreasRegistry, NavigationAreasRegistry>(Lifetime.Singleton);


        }

        private void AddNavigationComponents()
        {
            _container.RegisterOne2ManyTypesPerTypeUniqueness<ILayoutNavigationComponent, NavigationComponent<OrdinaryNavigationArea, NavigationAreaControl>>(Lifetime.Singleton)
                      .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutNavigationComponent, NavigationComponent<ContextualNavigationArea, ContextualNavigationAreaControl>>(Lifetime.Singleton);
        }

        #region DesignTime

        void IDesignTimeModule.Configure()
        {
            AddNavigationComponents();
            _container.RegisterType<INavigationAreasRegistry, NavigationAreasRegistry>(Lifetime.Singleton);
        }

        void IDesignTimeStandaloneWorkerModule.Run()
        {
            _timer = new Timer(OnTimer, null, 5000, 5000);
        }

        void IDesignTimeStandaloneWorkerModule.TryStop()
        {
            var timer = _timer;
            if (timer != null)
            {
                timer.Dispose();
            }
        }

        void IDesignTimeStandaloneWorkerModule.Wait()
        {
            throw new NotImplementedException();
        }
        
        private IContextualNavigationArea CurrentDocumentNavigation
        {
            get
            {
                var navigationManager = _container.Resolve<INavigationManager>();
                return (IContextualNavigationArea)navigationManager.Areas.SingleOrDefault(a => a is IContextualNavigationArea);
            }
        }
        
        private void OnTimer(object state)
        {
            var currentDocument = CurrentDocumentNavigation;
            if (currentDocument != null)
            {
                currentDocument.IsSelected = !currentDocument.IsSelected;
            }
        }

        #endregion
    }
}
