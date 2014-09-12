using System;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureQdsIndexing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            container
                .RegisterType<IEntityToDocumentRelationMetadataContainer, EntityToDocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IDocumentRelationMetadataContainer, DocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IEntityToDocumentRelationFactory, UnityEntityToDocumentRelationFactory>(lifetime())
                .RegisterType<IDocumentRelationFactory, UnityDocumentRelationFactory>(lifetime())
                .RegisterType<IDefferedDocumentUpdater, DefferedDocumentUpdater>(lifetime())
                .RegisterType<IDocumentUpdater, DocumentUpdater>(lifetime())
                .RegisterType(typeof(IDocumentVersionUpdater<>), typeof(DocumentVersionUpdater<>), lifetime());
                
            container.MassProcess(lifetime);

            return container;
        }

        private static void MassProcess(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            var entityToDocumentRelationMetadataMassProcessor = container.Resolve<EntityToDocumentRelationMetadataMassProcessor>();
            entityToDocumentRelationMetadataMassProcessor.MassProcess(lifetime);

            var documentRelationMetadataMassProcessor = container.Resolve<DocumentRelationMetadataMassProcessor>();
            documentRelationMetadataMassProcessor.MassProcess(lifetime);
        }
    }
}