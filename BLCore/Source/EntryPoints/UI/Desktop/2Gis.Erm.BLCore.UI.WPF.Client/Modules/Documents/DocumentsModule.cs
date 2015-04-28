using System;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Documents;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.Views.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Blendability;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents
{
    public sealed class DocumentsModule : IModule, IStandaloneWorkerModule, IDesignTimeStandaloneWorkerModule
    {
        private readonly IUnityContainer _container;

        public DocumentsModule(IUnityContainer container)
        {
            _container = container;
        }

        public Guid Id
        {
            get
            {
                return new Guid("B1C79D86-F479-4E2B-B7A4-EC652FFF4A68");
            }
        }

        public string Description
        {
            get
            {
                return "Erm WPF Client Document region infrastructure";
            }
        }

        public void Configure()
        {
            AddDocumentsComponents(_container);
            _container.RegisterType<IContextualDocumentProvider, ContextualDocumentProvider>(Lifetime.Singleton);
        }

        private void AddDocumentsComponents(IUnityContainer container)
        {
            container
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDocumentsComponent, DocumentComponent<CompositeDocumentViewModel, CompositeDocumentView>>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDocumentsComponent, DocumentComponent<ContextualDocumentViewModel, ContextualDocumentControl>>(Lifetime.Singleton)
                .RegisterOne2ManyTypesPerTypeUniqueness<ILayoutDocumentHeadersComponent, DocumentHeaderComponent<ContextualDocumentViewModel, ContextualDocumentHeaderControl>>(Lifetime.Singleton);
        }
        
        public void TryStop()
        {
        }

        public void Wait()
        {
        }

        public void Run()
        {
            var contextualDocumentProvider = _container.Resolve<IContextualDocumentProvider>();
            contextualDocumentProvider.AttachContextualDocument();
        }

        #region DesignTime

        void IDesignTimeModule.Configure()
        {
            AddDocumentsComponents(_container);
            _container.RegisterType<IContextualDocumentProvider, ContextualDocumentProvider>(Lifetime.Singleton);
        }

        void IDesignTimeStandaloneWorkerModule.Run()
        {
            _container.FillDocuments();
        }

        

        void IDesignTimeStandaloneWorkerModule.TryStop()
        {
            
        }

        void IDesignTimeStandaloneWorkerModule.Wait()
        {
            
        }

        #endregion
    }
}
