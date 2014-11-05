using System;
using System.Net.Http;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Settings;

using Elasticsearch.Net.Connection;
using Elasticsearch.Net.Connection.Thrift;

using Microsoft.Practices.Unity;

using Nest;

namespace DoubleGis.Erm.BLQuerying.DI.Config
{
    public static class QueryingBootstrapper
    {
        public static IUnityContainer ConfigureElasticApi(this IUnityContainer container, INestSettings nestSettings)
        {
            var elasticMetadataApi = new ElasticMetadataApi(nestSettings, IndexMappingMetadata.DocTypeToIndexNameMap);

            var client = new ElasticClient(nestSettings.ConnectionSettings, CreateConnection(nestSettings));
            var elasticApi = new ElasticApi(client, nestSettings, elasticMetadataApi);

            container.RegisterInstance<IElasticMetadataApi>(elasticMetadataApi, Lifetime.Singleton);
            container.RegisterInstance<IElasticApi>(elasticApi, Lifetime.Singleton);
            container.RegisterInstance<IElasticManagementApi>(elasticApi, Lifetime.Singleton);

            return container;
        }

        private static IConnection CreateConnection(INestSettings nestSettings)
        {
            switch (nestSettings.Protocol)
            {
                case Protocol.Http:
                    return new HttpClientConnection(nestSettings.ConnectionSettings, new WebRequestHandler
                    {
                        UseDefaultCredentials = true,
                        PreAuthenticate = true,
                    });
                case Protocol.Thrift:
                    return new ThriftConnection(nestSettings.ConnectionSettings);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
