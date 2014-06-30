using System;
using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticManagementApi
    {
        void Map<T>(Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> mappingSelector) where T : class;
        RootObjectMapping GetMapping<T>() where T : class;
        void DeleteIndex<T>() where T : class;
        void CreateIndex<T>(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector, string aliasName = null) where T : class;
        IndexSettings GetIndexSettings(Type indexType);
        void UpdateIndexSettings(Type[] indexTypes, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false);
    }

    public interface IElasticApi
    {
        T Get<T>(string id) where T : class;
        ICollection<IMultiGetHit<T>> MultiGet<T>(ICollection<string> ids) where T : class;

        void Delete<T>(Func<DeleteDescriptor<T>, DeleteDescriptor<T>> deleteSelector) where T : class;

        // FIXME {f.zaharov, 22.04.2014}: надо перейти полностью на strongly-typed индексацию
        void Index(object @object, Type type, string id);
        void Index<T>(T @object, Func<IndexDescriptor<T>, IndexDescriptor<T>> indexSelector = null) where T : class;

        IEnumerable<ICollection<T>> CreateBatches<T>(IEnumerable<T> items);
        ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;
        IEnumerable<T> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;
        IEnumerable<IHit<T>> Scroll2<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;

        void Bulk(IEnumerable<Func<BulkDescriptor, BulkDescriptor>> selectors);

        void Refresh(Func<RefreshDescriptor, RefreshDescriptor> refreshSelector = null);
    }
}