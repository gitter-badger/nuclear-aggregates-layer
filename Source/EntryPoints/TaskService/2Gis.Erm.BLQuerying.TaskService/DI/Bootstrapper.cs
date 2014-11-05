using System;

using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureQdsIndexing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            return container
                .RegisterType(typeof(IEntityToDocumentRelation<,>), typeof(EntityToDocumentRelation<,>), lifetime())
                .RegisterType(typeof(IEntityToDocumentRelation<User, UserAuthorizationDoc>), typeof(UserToUserAuthorizationDocRelation), lifetime())
                .RegisterType(typeof(IDocumentRelation<,>), typeof(DocumentRelation<,>), lifetime())
                .RegisterType<IEntityToDocumentRelationFactory, UnityEntityToDocumentRelationFactory>(lifetime())
                .RegisterType<IDocumentRelationFactory, UnityDocumentRelationFactory>(lifetime())
                .RegisterType<IDefferedDocumentUpdater, DefferedDocumentUpdater>(lifetime())
                .RegisterType<ReplicationQueueHelper>(lifetime())
                .RegisterType<IDocumentUpdater, DocumentUpdater>(lifetime())
                .RegisterType(typeof(IDocumentVersionUpdater<>), typeof(DocumentVersionUpdater<>), lifetime());
        }
    }
}