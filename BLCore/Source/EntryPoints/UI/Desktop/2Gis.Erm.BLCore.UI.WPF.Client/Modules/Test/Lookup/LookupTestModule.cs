using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Lookup.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Lookup.Views;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Lookup
{
    public class LookupTestModule : IStandaloneWorkerModule
    {
        private readonly IUnityContainer _container;

        public LookupTestModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get { return new Guid("AE9B22F7-7F99-494C-8840-A3D26DF69B5F"); }
        }

        public string Description
        {
            get { return "Lookup Test Module"; }
        }

        public void Configure()
        {
            _container.RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDocumentsComponent, DocumentComponent<LookupTestViewModel, LookupTestControl>>(Lifetime.Singleton);
        }

        public void Run()
        {
            // For debugging only
            var documentManager = _container.Resolve<IDocumentManager>();
            var document = new LookupTestViewModel()
            {
                Entity1Id = 1,
                Entity1Name = "First",

                Entity2Id = 2,
                Entity2Name = "Second"
            };

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
