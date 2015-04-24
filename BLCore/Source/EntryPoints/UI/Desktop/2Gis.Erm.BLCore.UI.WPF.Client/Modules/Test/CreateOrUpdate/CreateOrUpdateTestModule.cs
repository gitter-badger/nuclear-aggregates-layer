using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.CreateOrUpdate.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.CreateOrUpdate.Views;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.CreateOrUpdate
{
    public class CreateOrUpdateTestModule : IStandaloneWorkerModule
    {
        private readonly IUnityContainer _container;

        public CreateOrUpdateTestModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get { return new Guid("19752F61-8285-41E4-ADC1-EF8748759566"); }
        }

        public string Description
        {
            get { return "Create or Update Test Module"; }
        }

        public void Configure()
        {
            _container.RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDocumentsComponent, DocumentComponent<CreateOrUpdateTestViewModel, CreateOrUpdateTestControl>>(Lifetime.Singleton);
        }

        public void Run()
        {
            // For debugging only
            var documentManager = _container.Resolve<IDocumentManager>();
            var clientProxyFactory = _container.Resolve<IDesktopClientProxyFactory>();
            var configuration = _container.Resolve<IStandartConfigurationSettings>();
            var apiSetting = _container.Resolve<IApiSettings>();
            var document = new CreateOrUpdateTestViewModel(clientProxyFactory, configuration, apiSetting);

            documentManager.Add(document);
        }

        public void TryStop()
        {
            
        }

        public void Wait()
        {
            throw new NotImplementedException();
        }
    }
}
