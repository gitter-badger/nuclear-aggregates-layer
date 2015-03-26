using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Common.Settings;

using Elasticsearch.Net.ConnectionPool;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class CommonConnectionSettings : ConnectionSettings<CommonConnectionSettings>, IElasticMetadataApi
    {
        private readonly INestSettings _settings;

        public CommonConnectionSettings(INestSettings settings, IEnumerable<Tuple<Type, string>> docTypeToIndexNameMap)
            : base(GetConnectionPool(settings), null)
        {
            _settings = settings;

            ExposeRawResponse()                        // более подробные сообщения об ошибках
            .EnableCompressedResponses()                // accept-encoding: gzip, deflate
            .SetPingTimeout(2000)                       // на тестовом кластере живая нода может пинговаться долго, таймаут по умолчанию не подходит
            .ThrowOnElasticsearchServerExceptions();    // кидать исключения вместо выставления IResponse.IsValid

            foreach (var indexNameMapping in docTypeToIndexNameMap)
            {
                RegisterType(indexNameMapping.Item1, indexNameMapping.Item2);
            }
        }

        public void RegisterType<T>(string docIndexName, string docTypeName = null)
        {
            RegisterType(typeof(T), docIndexName, docTypeName);
        }

        private static IConnectionPool GetConnectionPool(INestSettings settings)
        {
            return new StaticConnectionPool(settings.Uris);
        }

        private string GetIsolatedIndexName(string indexName)
        {
            return _settings.IndexPrefix + "." + indexName.ToLowerInvariant();
        }

        private void RegisterType(Type documentType, string docIndexName, string docTypeName = null)
        {
            var isolatedIndexName = GetIsolatedIndexName(docIndexName);

            MapDefaultTypeIndices(defaultIndices =>
            {
                defaultIndices[documentType] = isolatedIndexName;
            });

            MapDefaultTypeNames(defaultTypeNames =>
            {
                if (docTypeName != null)
                {
                    defaultTypeNames[documentType] = docTypeName.ToLowerInvariant();
                }
            });
        }
    }
}