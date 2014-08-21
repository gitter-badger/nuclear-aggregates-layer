using System;
using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticMetadataApi
    {
        void RegisterType<T>(string docIndexName, string docTypeName = null);
        string GetIsolatedIndexName(string indexName);
    }

    public interface IElasticManagementApi
    {
        void Map<T>(Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> mappingSelector) where T : class;
        void DeleteIndex<T>() where T : class;
        void CreateIndex<T>(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector) where T : class;
        void AddAlias<T>(string alias) where T : class;
        IndexSettings GetIndexSettings(Type indexType);
        void UpdateIndexSettings(Type indexType, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false);
        void DeleteMapping<T>() where T : class;

        bool TypeExists<T>() where T : class;
    }

    public interface IElasticApi
    {
        T Get<T>(string id) where T : class;
        IReadOnlyCollection<IMultiGetHit<object>> MultiGet(Func<ElasticApi.ErmMultiGetDescriptor, ElasticApi.ErmMultiGetDescriptor> multiGetSelector);

        void Index<T>(T @object, Func<IndexDescriptor<T>, IndexDescriptor<T>> indexSelector = null) where T : class;
        void Delete<T>(string id) where T : class;

        ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;
        IEnumerable<IHit<T>> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;

        IEnumerable<IReadOnlyCollection<T>> CreateBatches<T>(IEnumerable<T> items);
        void Bulk(IReadOnlyCollection<Func<ElasticApi.ErmBulkDescriptor, ElasticApi.ErmBulkDescriptor>> selectors);

        void Refresh<T>() where T : class;
        void Refresh(Type[] indexTypes);
    }
}