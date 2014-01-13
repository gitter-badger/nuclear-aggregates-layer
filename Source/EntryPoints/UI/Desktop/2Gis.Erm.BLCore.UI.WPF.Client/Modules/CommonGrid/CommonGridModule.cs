using System;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Old;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.CommonGrid
{
    public class CommonGridModule : IModule, IDesignTimeModule
    {
        private readonly IUnityContainer _container;

        public CommonGridModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get { return new Guid("50CC36AC-B6B3-47B8-83DC-8EE54E8F3139"); }
        }

        public string Description
        {
            get { return "Common Grid Module"; }
        }
            
        public void Configure()
        {
            _container.RegisterType<IUIConfigurationService, UIConfigurationService>(Lifetime.Singleton)
                // TODO {all, 06.08.2013}: подумать как более корректно получать доступ к IGlobalizationSettings
                      .RegisterType<IGlobalizationSettings>(Lifetime.Singleton, new InjectionFactory((container, type, arg3) => container.Resolve<ICommonSettings>()))
                      .RegisterType<IGridStructuresProvider, GridStructuresProvider>();

        }

        #region Design time
        void IDesignTimeModule.Configure()
        {
            _container
                // TODO {all, 06.08.2013}: подумать как более корректно получать доступ к IGlobalizationSettings
                .RegisterType<IGlobalizationSettings>(Lifetime.Singleton,
                                                           new InjectionFactory((container, type, arg3) => container.Resolve<ICommonSettings>()))
                // .RegisterType<IListNonGenericEntityService, RestApiListNonGenericEntityService>(Lifetime.Singleton)
                .RegisterType<IApiClient, NullApiClient>(Lifetime.Singleton);
        }
        #endregion
    }
}
