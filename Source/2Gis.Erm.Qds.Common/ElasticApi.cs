using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class ElasticApi : IElasticApi
    {
        private readonly IElasticConnectionSettingsFactory _elasticConnectionSettingsFactory;
        private readonly IElasticClientFactory _elasticClientFactory;

        public ElasticApi(IElasticConnectionSettingsFactory elasticConnectionSettingsFactory, IElasticClientFactory elasticClientFactory)
        {
            _elasticConnectionSettingsFactory = elasticConnectionSettingsFactory;
            _elasticClientFactory = elasticClientFactory;
        }

        string IElasticApi.GetIsolatedIndexName(string indexName)
        {
            return _elasticConnectionSettingsFactory.GetIsolatedIndexName(indexName);
        }

        void IElasticApi.Bulk(IEnumerable<BulkDescriptor> bulkDescriptors)
        {
            _elasticClientFactory.UsingElasticClient(client => BulkInternal(client, bulkDescriptors));
        }

        void IElasticApi.BulkExclusive<TDocument>(IEnumerable<BulkDescriptor> bulkDescriptors)
        {
            var indexName = DocumentMetadata.IndexNameMappings[typeof(TDocument)];
            var isolatedIndexName = _elasticConnectionSettingsFactory.GetIsolatedIndexName(indexName);

            _elasticClientFactory.UsingElasticClient(client =>
            {
                client.UpdateSettings(isolatedIndexName, new IndexSettings { { "number_of_replicas", 0 } });

                try
                {
                    BulkInternal(client, bulkDescriptors);
                }
                finally
                {
                    client.UpdateSettings(isolatedIndexName, new IndexSettings { { "number_of_replicas", 1 } });
                }
            });
        }

        private static void BulkInternal(IElasticClient client, IEnumerable<BulkDescriptor> bulkDescriptors)
        {
            foreach (var bulkDescriptor in bulkDescriptors)
            {
                // refresh data after bulk operation
                bulkDescriptor.Refresh();

                var response = client.Bulk(bulkDescriptor);
                if (response.Items.Any(x => !x.OK))
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}