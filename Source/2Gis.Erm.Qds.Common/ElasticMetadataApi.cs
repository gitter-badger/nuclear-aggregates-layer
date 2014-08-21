using System;

using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.Common.Settings;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class ElasticMetadataApi : IElasticMetadataApi
    {
        private readonly INestSettings _nestSettings;

        public ElasticMetadataApi(INestSettings nestSettings)
        {
            _nestSettings = nestSettings;
            RegisterKnownTypes();
        }

        public void RegisterKnownTypes()
        {
            foreach (var indexNameMapping in IndexMappingMetadata.DocTypeToIndexNameMap)
            {
                RegisterType(indexNameMapping.Item1, indexNameMapping.Item2);
            }
        }

        public void RegisterType<T>(string docIndexName, string docTypeName = null)
        {
            RegisterType(typeof(T), docIndexName, docTypeName);
        }

        private void RegisterType(Type documentType, string docIndexName, string docTypeName = null)
        {
            var isolatedIndexName = GetIsolatedIndexName(docIndexName);
            _nestSettings.ConnectionSettings.DefaultIndices.Add(documentType, isolatedIndexName);

            if (docTypeName != null)
            {
                _nestSettings.ConnectionSettings.DefaultTypeNames.Add(documentType, docTypeName.ToLowerInvariant());
            }
        }

        public string GetIsolatedIndexName(string indexName)
        {
            return _nestSettings.IndexPrefix + "." + indexName.ToLowerInvariant();
        }
    }
}