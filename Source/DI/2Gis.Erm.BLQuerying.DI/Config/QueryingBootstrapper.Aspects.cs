using System;

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
            var elasticMetadataApi = new ElasticMetadataApi(nestSettings);

            var client = new ElasticClient(nestSettings.ConnectionSettings, CreateConnection(nestSettings));
            var elasticApi = new ElasticApi(client, nestSettings, elasticMetadataApi);

            container.RegisterInstance<IElasticMetadataApi>(elasticMetadataApi);
            container.RegisterInstance<IElasticApi>(elasticApi);
            container.RegisterInstance<IElasticManagementApi>(elasticApi);

            return container;
        }

        private static IConnection CreateConnection(INestSettings nestSettings)
        {
            switch (nestSettings.Protocol)
            {
                case Protocol.Http:
                    return new WindowsAuthHttpConnection(nestSettings.ConnectionSettings);
                case Protocol.Thrift:
                    return new ThriftConnection(nestSettings.ConnectionSettings);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
