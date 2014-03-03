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

        public static IElasticClient CreateIndex(this IElasticClient elastic, string indexName)
        {
            elastic.CreateIndex(indexName.ToLowerInvariant(), idescr => idescr);

            return elastic;
        }

        public static IElasticClient DeleteIndex(this IElasticClient elastic, string indexName)
        {
            elastic.DeleteIndex(indexName.ToLowerInvariant());

            return elastic;
        }
    }
}