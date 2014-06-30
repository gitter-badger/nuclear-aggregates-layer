using System;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common;

using Elasticsearch.Net.Connection;

using Moq;

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

            var connectionStringSettings = new Mock<IConnectionStringSettings>();
            connectionStringSettings.Setup(x => x.GetConnectionString(ConnectionStringName.ErmSearch)).Returns(connectionString);

            return new NestSettingsAspect(connectionStringSettings.Object);
        }

        public static IElasticApi CreateElasticApi(INestSettings nestSettings)
        {
            var connection = new HttpConnection(nestSettings.ConnectionSettings);
            var elasticClient = new ElasticClient(nestSettings.ConnectionSettings, connection);
            return new ElasticApi(elasticClient, nestSettings, new ElasticResponseHandler());
        }

        public static Func<CreateIndexDescriptor, CreateIndexDescriptor> GetTestIndexDescriptor()
        {
            return x => x
            .NumberOfShards(1)
            .NumberOfReplicas(1)
            .Settings(s => s.Add("refresh_interval", -1));
        }

        public static void RegisterDocumentAndCreateIndex<T>(INestSettings nestSettings, IElasticManagementApi elasticManagementApi) where T: class
        {
            nestSettings.RegisterType<T>(TestIndexName);
            elasticManagementApi.CreateIndex<T>(GetTestIndexDescriptor());
        }
    }
}