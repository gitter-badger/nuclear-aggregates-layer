using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Common.Settings;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class ElasticMetadataApi : IElasticMetadataApi
    {
        private readonly INestSettings _nestSettings;

        public ElasticMetadataApi(INestSettings nestSettings, IEnumerable<Tuple<Type, string>> docTypeToIndexNameMap)
        {
            _nestSettings = nestSettings;
            RegisterKnownTypes(docTypeToIndexNameMap);
        }

        private FluentDictionary<Type, string> DefaultIndices
        {
            get { return _nestSettings.ConnectionSettings.DefaultIndices; }
        }

        private FluentDictionary<Type, string> DefaultTypeNames
        {
            get { return _nestSettings.ConnectionSettings.DefaultTypeNames; }
        }

        public string GetIsolatedIndexName(string indexName)
        {
            return _nestSettings.IndexPrefix + "." + indexName.ToLowerInvariant();
        }

        public void RegisterType<T>(string docIndexName, string docTypeName)
        {
            RegisterType(typeof(T), docIndexName, docTypeName);
        }

        private void RegisterKnownTypes(IEnumerable<Tuple<Type, string>> docTypeToIndexNameMap)
        {
            foreach (var indexNameMapping in docTypeToIndexNameMap)
            {
                RegisterType(indexNameMapping.Item1, indexNameMapping.Item2);
            }
        }
        
        private void RegisterType(Type documentType, string docIndexName, string docTypeName = null)
        {
            var isolatedIndexName = GetIsolatedIndexName(docIndexName);
            if (!DefaultIndices.ContainsKey(documentType))
            {
                DefaultIndices.Add(documentType, isolatedIndexName);
            }

            if (!DefaultTypeNames.ContainsKey(documentType) && docTypeName != null)
            {
                DefaultTypeNames.Add(documentType, docTypeName.ToLowerInvariant());
            }
        }
    }
}