using System;

using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Common.Settings;

using Elasticsearch.Net.Connection;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    public static class ElasticTestConfigurator
    {
        private const string TestIndexPrefix = "Test.Integration";
        private const string TestIndexName = "Data";

        public static INestSettings CreateSettings(string host)
        {
            var connectionString = "Uris=['http://" + host + ":9200'];Protocol=http;IndexPrefix=" + TestIndexPrefix + ";BatchSize=10000";
            return new NestSettingsAspect(connectionString);
        }

        public static IElasticApi CreateElasticApi(INestSettings nestSettings)
        {
            var connection = new HttpConnection(nestSettings.ConnectionSettings);
            var elasticClient = new ElasticClient(nestSettings.ConnectionSettings, connection);
            return new ElasticApi(elasticClient, nestSettings, null);
        }

        public static Func<CreateIndexDescriptor, CreateIndexDescriptor> GetTestIndexDescriptor()
        {
            return x => x
            .NumberOfShards(1)
            .NumberOfReplicas(1)
            .Settings(s => s.Add("refresh_interval", -1));
        }

        public static void RegisterDocumentAndCreateIndex<T>(IElasticMetadataApi metadataApi, IElasticManagementApi managementApi) where T : class
        {
            metadataApi.RegisterType<T>(TestIndexName);
            managementApi.CreateIndex<T>(GetTestIndexDescriptor());
        }
    }
}