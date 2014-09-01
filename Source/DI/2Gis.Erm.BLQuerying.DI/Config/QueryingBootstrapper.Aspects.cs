using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Settings;
using DoubleGis.Erm.Qds.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Listing;

using Elasticsearch.Net.Connection;
using Elasticsearch.Net.Connection.Thrift;

using Microsoft.Practices.Unity;

using Nest;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static partial class QueryingBootstrapper
    {
        private static readonly EntityName[] QdsEntityNames =
        {
            EntityName.Order
        };

        public static IUnityContainer ConfigureQds(this IUnityContainer container, Func<LifetimeManager> lifetime, INestSettings nestSettings)
        {
            container
                .ConfigureElasticApi(nestSettings)
                .ConfigureQdsIndexing(lifetime);

            container.MassProcess(lifetime);

            return container;
        }

        private static IUnityContainer ConfigureElasticApi(this IUnityContainer container, INestSettings nestSettings)
        {
            switch (nestSettings.Protocol)
            {
                case Protocol.Http:
                    {
                        container.RegisterType<IConnection, WindowsAuthHttpConnection>(Lifetime.Singleton, new InjectionConstructor(nestSettings.ConnectionSettings));
                    }
                    break;
                case Protocol.Thrift:
                    {
                        container.RegisterType<IConnection, ThriftConnection>(Lifetime.Singleton, new InjectionConstructor(nestSettings.ConnectionSettings));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            container.RegisterType<IElasticMetadataApi, ElasticMetadataApi>(Lifetime.Singleton);
            container.RegisterType<IElasticApi, ElasticApi>(Lifetime.Singleton, new InjectionFactory(x =>
            {
                var connection = x.Resolve<IConnection>();
                var client = new ElasticClient(nestSettings.ConnectionSettings, connection);
                var metadataApi = x.Resolve<IElasticMetadataApi>();
                return new ElasticApi(client, nestSettings, metadataApi);
            }));
            container.RegisterType<IElasticManagementApi, ElasticApi>(Lifetime.Singleton);
            return container;
        }

        private static IUnityContainer ConfigureQdsIndexing(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            container
                .RegisterType<IEntityToDocumentRelationMetadataContainer, EntityToDocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IDocumentRelationMetadataContainer, DocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IEntityToDocumentRelationFactory, UnityEntityToDocumentRelationFactory>(lifetime())
                .RegisterType<IDocumentRelationFactory, UnityDocumentRelationFactory>(Lifetime.Singleton)
                .RegisterType<IDefferedDocumentUpdater, DefferedDocumentUpdater>(lifetime())
                .RegisterType<IDocumentUpdater, DocumentUpdater>(lifetime())
                .RegisterType(typeof(IDocumentVersionUpdater<>), typeof(DocumentVersionUpdater<>), Lifetime.Singleton)
                ;

            return container;
        }

        private static void MassProcess(this IUnityContainer container, Func<LifetimeManager> lifetime)
        {
            var entityToDocumentRelationMetadataMassProcessor = container.Resolve<EntityToDocumentRelationMetadataMassProcessor>();
            entityToDocumentRelationMetadataMassProcessor.MassProcess(lifetime);

            var documentRelationMetadataMassProcessor = container.Resolve<DocumentRelationMetadataMassProcessor>();
            documentRelationMetadataMassProcessor.MassProcess();
        }

        public static Type ListServiceConflictResolver(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            var entityName = entitySet.Entities.Single();
            if (QdsEntityNames.Contains(entityName))
            {
                var businessModel = ConfigFileSetting.Enum.Required<BusinessModel>("BusinessModel").Value;
                if (businessModel == BusinessModel.Russia)
                {
                    return candidates.Single(x => x.Assembly == typeof(QdsListOrderService).Assembly);
                }

                return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
            }

            return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
        }
    }
}
