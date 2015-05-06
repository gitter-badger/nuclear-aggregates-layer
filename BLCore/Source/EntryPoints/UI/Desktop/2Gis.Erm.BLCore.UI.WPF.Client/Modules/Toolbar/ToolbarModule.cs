using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Toolbar
{
    public sealed class ToolbarModule : IModule, IDesignTimeModule
    {
        private readonly IUnityContainer _container;

        public ToolbarModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get { return new Guid("916A9DE3-F4EA-43F2-A92C-B97CACE53A6E"); }
        }

        public string Description
        {
            get { return "Classic Toolbar module"; }
        }

        public void Configure()
        {
            _container
                .RegisterType<IToolbar, ToolbarViewModel>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutToolbarComponent, ToolbarComponent<ToolbarViewModel, ToolbarView>>(Lifetime.Singleton);
        }

        #region Design time

        void IDesignTimeModule.Configure()
        {
            _container
                .RegisterType<IToolbar, ToolbarViewModel>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutToolbarComponent, ToolbarComponent<ToolbarViewModel, ToolbarView>>(Lifetime.Singleton);
        }

        #endregion
    }
}
