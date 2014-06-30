using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Core.Settings;

using Elasticsearch.Net;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    // COMMENT {m.pashuk, 23.04.2014}: Идея с названиями: NestElasticApi : IStorageElasticApi, IConfigElasticApi
    public sealed class ElasticApi : IElasticApi, IElasticManagementApi
    {
        private readonly IElasticClient _elasticClient;

        private readonly INestSettings _nestSettings;
        private readonly IElasticResponseHandler _responseHandler;

        public ElasticApi(IElasticClient elasticClient, INestSettings nestSettings, IElasticResponseHandler responseHandler)
        {
            _elasticClient = elasticClient;

            _nestSettings = nestSettings;
            _responseHandler = responseHandler;
        }

        public void Bulk(IEnumerable<Func<BulkDescriptor, BulkDescriptor>> selectors)
        {
            var batches = CreateBatches(selectors);
            var aggregatedFuncs = batches.Select(batch => new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor =>
            {
                foreach (var func in batch)
                {
                    bulkDescriptor = func(bulkDescriptor);
                }

                return bulkDescriptor;
            }));

            foreach (var aggregatedFunc in aggregatedFuncs)
            {
                Bulk(aggregatedFunc);
            }
        }

        private void Bulk(Func<BulkDescriptor, BulkDescriptor> bulkSelector)
        {
            var response = _elasticClient.Bulk(bulkSelector);
            _responseHandler.ThrowWhenError(response);
        }

        public IEnumerable<ICollection<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            var buffer = new List<T>(_nestSettings.BatchSize);

            foreach (var item in items)
            {
                buffer.Add(item);

                if (buffer.Count == buffer.Capacity)
                {
                    yield return buffer;
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                yield return buffer;
            }
        }

        public ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
        {
            var response = _elasticClient.Search(searcher);
            _responseHandler.ThrowWhenError(response);
            return response;
        }

        public void Delete<T>(Func<DeleteDescriptor<T>, DeleteDescriptor<T>> deleteSelector) where T : class
        {
            var response = _elasticClient.Delete(deleteSelector);
            _responseHandler.ThrowWhenError(response);
        }

        public void Index<T>(T @object, Func<IndexDescriptor<T>, IndexDescriptor<T>> indexSelector = null) where T : class
        {
            var response = _elasticClient.Index(@object, indexSelector);
            _responseHandler.ThrowWhenError(response);
        }

        public T Get<T>(string id) where T : class
        {
            var response = _elasticClient.Get<T>(x => x.Id(id));
            _responseHandler.ThrowWhenError(response);
            return response.Source;
        }

        public ICollection<IMultiGetHit<T>> MultiGet<T>(ICollection<string> ids) where T : class
        {
            var response = _elasticClient.MultiGet(x => x.GetMany<T>(ids));
            _responseHandler.ThrowWhenError(response);
            return response.GetMany<T>(ids).ToArray();
        }

        public void Index(object @object, Type type, string id)
        {
            string isolatedIndexName;
            if (!_nestSettings.ConnectionSettings.DefaultIndices.TryGetValue(type, out isolatedIndexName))
            {
                throw new ArgumentException("Cannot find index name for type " + type.Name);
            }

            var response = _elasticClient.Index(@object, x => x.Index(isolatedIndexName).Type(type).Id(id));
            _responseHandler.ThrowWhenError(response);
        }

        public void Map<T>(Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> mappingSelector) where T : class
        {
            var response = _elasticClient.Map(mappingSelector);
            _responseHandler.ThrowWhenError(response);
        }

        public RootObjectMapping GetMapping<T>() where T : class
        {
            var indexExists = IndexExists<T>();
            if (!indexExists)
            {
                return null;
            }

            var getMappingResponse = _elasticClient.GetMapping(x => x.Index<T>().Type<T>());
            _responseHandler.ThrowWhenError(getMappingResponse);
            return getMappingResponse.Mapping;
        }

        public void Refresh(Func<RefreshDescriptor, RefreshDescriptor> refreshSelector = null)
        {
            var response = _elasticClient.Refresh(refreshSelector);
            _responseHandler.ThrowWhenError(response);
        }

        private ISearchResponse<T> Scroll<T>(Func<ScrollDescriptor<T>, ScrollDescriptor<T>> scrollSelector) where T : class
        {
            var response = _elasticClient.Scroll(scrollSelector);
            _responseHandler.ThrowWhenError(response);
            return response;
        }

        public void DeleteIndex<T>() where T : class
        {
            var indexExists = IndexExists<T>();
            if (!indexExists)
            {
                return;
            }

            var deleteIndexResponse = _elasticClient.DeleteIndex(x => x.Index<T>());
            _responseHandler.ThrowWhenError(deleteIndexResponse);
        }

        public void CreateIndex<T>(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector, string aliasName = null) where T : class
        {
            var indexExists = IndexExists<T>();
            if (indexExists)
            {
                return;
            }

            var key = typeof(T);
            string isolatedIndexName;
            if (!_nestSettings.ConnectionSettings.DefaultIndices.TryGetValue(key, out isolatedIndexName))
            {
                throw new ArgumentException("Cannot find index name for type " + key.Name);
            }

            var resultIndexSelector = createIndexSelector;

            if (!string.IsNullOrEmpty(aliasName))
            {
                var isolatedAliasName = _nestSettings.GetIsolatedIndexName(aliasName);
                resultIndexSelector = x => createIndexSelector(x).AddAlias(isolatedAliasName.ToLowerInvariant());
            }

            var response = _elasticClient.CreateIndex(isolatedIndexName, resultIndexSelector);
            _responseHandler.ThrowWhenError(response);
        }

        private bool IndexExists<T>() where T : class
        {
            var indexExistsResponse = _elasticClient.IndexExists(x => x.Index<T>());
            _responseHandler.ThrowWhenError(indexExistsResponse);
            return indexExistsResponse.Exists;
        }

        public IndexSettings GetIndexSettings(Type indexType)
        {
            var response = _elasticClient.GetIndexSettings(x => x.Index(indexType));
            _responseHandler.ThrowWhenError(response);
            return response.Settings;
        }

        public void UpdateIndexSettings(Type[] indexTypes, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false)
        {
            foreach (var indexType in indexTypes)
            {
                var type = indexType;
                var response = _elasticClient.UpdateSettings(x => updateSettingsSelector(x).Index(type));
                _responseHandler.ThrowWhenError(response);
            }

            if (optimize)
            {
                _elasticClient.Optimize(x => x.Indices(indexTypes).MaxNumSegments(5));
            }
        }

        public IEnumerable<T> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
        {
            return new DelegateEnumerable<T>(() => new ScrollEnumerator<T>(this, searcher));
        }

        public IEnumerable<IHit<T>> Scroll2<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
        {
            return new DelegateEnumerable<IHit<T>>(() => new ScrollEnumerator2<T>(this, searcher));
        }

        private sealed class DelegateEnumerable<TDocument> : IEnumerable<TDocument>
            where TDocument : class
        {
            private readonly Func<IEnumerator<TDocument>> _func;

            public DelegateEnumerable(Func<IEnumerator<TDocument>> func)
            {
                _func = func;
            }

            public IEnumerator<TDocument> GetEnumerator()
            {
                return _func();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ScrollEnumerator<TDocument> : IEnumerator<TDocument>
            where TDocument : class
        {
            private readonly ElasticApi _elasticApi;
            private readonly Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> _searcher;
            private string _scrollId;
            private IEnumerator<TDocument> _internalEnumerator;

            public ScrollEnumerator(ElasticApi elasticApi, Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> searcher)
            {
                const string FirstScrollTimeout = "1s";

                _elasticApi = elasticApi;

                _searcher = searchDescriptor =>
                    searcher(searchDescriptor)
                    .SearchType(SearchTypeOptions.Scan)
                    .Scroll(FirstScrollTimeout)
                    .Size(_elasticApi._nestSettings.BatchSize);
            }

            public TDocument Current { get { return _internalEnumerator.Current; } }
            object IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                if (_scrollId == null)
                {
                    var response = _elasticApi.Search(_searcher);
                    if (response.Total <= 0)
                    {
                        return false;
                    }
                    _scrollId = response.ScrollId;
                }

                if (_internalEnumerator == null || !_internalEnumerator.MoveNext())
                {
                    var response = _elasticApi.Scroll<TDocument>(x => x.Scroll(_elasticApi._nestSettings.BatchTimeout).ScrollId(_scrollId));
                    _scrollId = response.ScrollId;
                    _internalEnumerator = response.Documents.GetEnumerator();
                    return _internalEnumerator.MoveNext();
                }

                return true;
            }

            public void Dispose()
            {
                if (_internalEnumerator != null)
                {
                    _internalEnumerator.Dispose();
                    _internalEnumerator = null;
                }

                _scrollId = null;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        private sealed class ScrollEnumerator2<TDocument> : IEnumerator<IHit<TDocument>>
            where TDocument : class
        {
            private readonly ElasticApi _elasticApi;
            private readonly Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> _searcher;
            private string _scrollId;
            private IEnumerator<IHit<TDocument>> _internalEnumerator;

            public ScrollEnumerator2(ElasticApi elasticApi, Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> searcher)
            {
                const string FirstScrollTimeout = "1s";

                _elasticApi = elasticApi;

                _searcher = searchDescriptor =>
                    searcher(searchDescriptor)
                    .SearchType(SearchTypeOptions.Scan)
                    .Scroll(FirstScrollTimeout)
                    .Size(_elasticApi._nestSettings.BatchSize);
            }

            public IHit<TDocument> Current { get { return _internalEnumerator.Current; } }
            object IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                if (_scrollId == null)
                {
                    var response = _elasticApi.Search(_searcher);
                    if (response.Total <= 0)
                    {
                        return false;
                    }
                    _scrollId = response.ScrollId;
                }

                if (_internalEnumerator == null || !_internalEnumerator.MoveNext())
                {
                    var response = _elasticApi.Scroll<TDocument>(x => x.Scroll(_elasticApi._nestSettings.BatchTimeout).ScrollId(_scrollId));
                    _scrollId = response.ScrollId;
                    _internalEnumerator = response.Hits.GetEnumerator();
                    return _internalEnumerator.MoveNext();
                }

                return true;
            }

            public void Dispose()
            {
                if (_internalEnumerator != null)
                {
                    _internalEnumerator.Dispose();
                    _internalEnumerator = null;
                }

                _scrollId = null;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }
    }
}