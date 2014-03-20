using System;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    public static class ElasticTestConfigurator
    {
        public const string TestIndexName = "test.index.elastic";

        public static IElasticClient CreateElasticClient(string host)
        {
            var settings = new ConnectionSettings(new Uri(host));
            return new ElasticClient(settings);
        }
    }
}