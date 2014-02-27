using System;

using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations;

using Nest;

namespace DoubleGis.Erm.Qds.Common.ElasticClient
{
    public sealed class ElasticConnectionSettingsFactory : IElasticConnectionSettingsFactory
    {
        private readonly ISearchSettings _searchSettings;

        public ElasticConnectionSettingsFactory(ISearchSettings searchSettings)
        {
            _searchSettings = searchSettings;
        }

        public IConnectionSettings CreateConnectionSettings(Uri uri)
        {
            var connectionSettings = new ConnectionSettings(uri)
            .MapDefaultTypeIndices(x =>
            {
                foreach (var indexNameMapping in DocumentMetadata.IndexNameMappings)
                {
                    var isolatedIndexName = GetIsolatedIndexName(indexNameMapping.Value);
                    x.Add(indexNameMapping.Key, isolatedIndexName);
                }
            });

            return connectionSettings;
        }

        public string GetIsolatedIndexName(string indexName)
        {
            return string.Concat(_searchSettings.IndexPrefix, ".", indexName);
        }
    }
}