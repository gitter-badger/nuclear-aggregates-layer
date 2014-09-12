using System;

using DoubleGis.Erm.Qds.API.Operations.Docs.Metadata;
using DoubleGis.Erm.Qds.Common.Settings;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    // FIXME {m.pashuk, 04.09.2014}:  ласс измен€ет состо€ние своей зависимости. ѕон€ть действительно ли это нужно и, если да, то завести специальную абстракцию над INestSettings 
    public sealed class ElasticMetadataApi : IElasticMetadataApi
    {
        private readonly INestSettings _nestSettings;

        public ElasticMetadataApi(INestSettings nestSettings)
        {
            _nestSettings = nestSettings;
            RegisterKnownTypes();
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

        private void RegisterKnownTypes()
        {
            foreach (var indexNameMapping in IndexMappingMetadata.DocTypeToIndexNameMap)
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