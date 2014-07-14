using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Elastic.Nest.Qds;
using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl;
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
        public static IUnityContainer ConfigureQds(this IUnityContainer container, Func<LifetimeManager> lifetime, INestSettings nestSettings)
        {
            container
                .ConfigureElasticApi(nestSettings)
                .ConfigureQdsIndexing();

            container.MassProcess();

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

            container.RegisterType<IElasticResponseHandler, ElasticResponseHandler>(Lifetime.Singleton);
            container.RegisterType<INestSerializer, NestSerializer>(Lifetime.Singleton, new InjectionConstructor(nestSettings.ConnectionSettings));
            container.RegisterType<IElasticClient, ElasticClient>(Lifetime.Singleton, new InjectionConstructor(nestSettings.ConnectionSettings, typeof(IConnection), typeof(INestSerializer)));
            container.RegisterType<IElasticApi, ElasticApi>(Lifetime.Singleton);
            container.RegisterType<IElasticManagementApi, ElasticApi>(Lifetime.Singleton);
            return container;
        }

        private static IUnityContainer ConfigureQdsIndexing(this IUnityContainer container)
        {
            container
                .RegisterType<IEntityToDocumentRelationMetadataContainer, EntityToDocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IDocumentRelationMetadataContainer, DocumentRelationMetadataContainer>(Lifetime.Singleton)
                .RegisterType<IDocumentRelationFactory, UnityDocumentRelationFactory>(Lifetime.Singleton)
                .RegisterType<IDocumentUpdaterFactory, UnityDocumentUpdaterFactory>(Lifetime.Singleton)
                .RegisterType<IDefferedDocumentUpdater, DefferedDocumentUpdater>(Lifetime.Singleton)
                .RegisterType(typeof(DocumentUpdater<,>), Lifetime.Singleton)
                .RegisterType(typeof(IDocumentPartUpdater<>), typeof(DocumentPartUpdater<>), Lifetime.Singleton);

            return container;
        }

        // TODO {m.pashuk, 16.05.2014}: поместить в правильное место
        private static void MassProcess(this IUnityContainer container)
        {
            var entityToDocumentRelationMetadataMassProcessor = container.Resolve<EntityToDocumentRelationMetadataMassProcessor>();
            entityToDocumentRelationMetadataMassProcessor.MassProcess();

            var documentRelationMetadataMassProcessor = container.Resolve<DocumentRelationMetadataMassProcessor>();
            documentRelationMetadataMassProcessor.MassProcess();
        }

        public static Type ListServiceConflictResolver(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            if (entitySet.Entities.Contains(EntityName.Order))
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
