using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubleGis.Erm.Qds.Common.Settings;
using Elasticsearch.Net;
using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    // COMMENT {m.pashuk, 23.04.2014}: Идея с названиями: NestElasticApi : IStorageElasticApi, IConfigElasticApi
    public sealed class ElasticApi : IElasticApi, IElasticManagementApi
    {
        private readonly IElasticClient _elasticClient;
        private readonly INestSettings _nestSettings;
        private readonly IElasticMetadataApi _metadataApi;
        
        public ElasticApi(IElasticClient elasticClient, INestSettings nestSettings, IElasticMetadataApi metadataApi)
        {
            _elasticClient = elasticClient;
            _nestSettings = nestSettings;
            _metadataApi = metadataApi;
        }

        // TODO {m.pashuk, 05.08.2014}: message https://github.com/elasticsearch/elasticsearch-net/issues/504
        public bool TypeExists<T>() where T : class
        {
            var documentType = typeof(T);

            string isolatedIndexName;
            if (!_nestSettings.ConnectionSettings.DefaultIndices.TryGetValue(documentType, out isolatedIndexName))
            {
                throw new ArgumentException("Cannot find index name for type " + documentType.Name);
            }

            var documentTypeName = documentType.Name.ToLowerInvariant();
            var response = _elasticClient.Raw.IndicesExistsType<VoidResponse>(isolatedIndexName, documentTypeName);
            return response.Success;
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
            var bulkResponse = _elasticClient.Bulk(bulkSelector);
            if (!bulkResponse.Errors)
            {
                return;
            }

            var firstError = bulkResponse.ItemsWithErrors.First().Error;
            throw new ApplicationException(firstError);
        }

        public IEnumerable<IReadOnlyCollection<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            var buffer = new List<T>(_nestSettings.BatchSize);

            foreach (var item in items)
            {
                buffer.Add(item);

                if (buffer.Count == buffer.Capacity)
                {
                    yield return buffer;
                    buffer = new List<T>(_nestSettings.BatchSize);
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
            return response;
        }

        public void Delete<T>(string id) where T : class
        {
            _elasticClient.Delete<T>(x => x.Id(id));
        }

        public void Index<T>(T @object, Func<IndexDescriptor<T>, IndexDescriptor<T>> indexSelector = null) where T : class
        {
            _elasticClient.Index(@object, indexSelector);
        }

        public T Get<T>(string id) where T : class
        {
            var response = _elasticClient.Get<T>(x => x.Id(id));
            return response.Source;
        }

        public IReadOnlyCollection<IMultiGetHit<object>> MultiGet(Func<MultiGetDescriptor2, MultiGetDescriptor2> multiGetSelector)
        {
            var response = _elasticClient.MultiGet(x => multiGetSelector(new MultiGetDescriptor2()));
            var documents = (IReadOnlyCollection<IMultiGetHit<object>>)response.Documents;
            return documents;
        }

        public void Map<T>(Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> mappingSelector) where T : class
        {
            _elasticClient.Map(mappingSelector);
        }

        public void DeleteMapping<T>() where T : class
        {
            _elasticClient.DeleteMapping<T>();
        }

        public void Refresh<T>() where T : class
        {
            _elasticClient.Refresh(x => x.Index<T>());
        }

        public void Refresh(Type[] indexTypes)
        {
            _elasticClient.Refresh(x => x.Indices(indexTypes));
        }

        public void DeleteIndex<T>() where T : class
        {
            var indexExists = IndexExists<T>();
            if (!indexExists)
            {
                return;
            }

            _elasticClient.DeleteIndex(x => x.Index<T>());
        }

        public void CreateIndex<T>(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector) where T : class
        {
            var indexExists = IndexExists<T>();
            if (indexExists)
            {
                return;
            }

            _elasticClient.CreateIndex(x => createIndexSelector(x).Index<T>());
        }

        public void AddAlias<T>(string alias) where T : class
        {
            var isolatedIndexName = _metadataApi.GetIsolatedIndexName(alias);
            _elasticClient.Alias(x => x.Add(y => y.Index<T>().Alias(isolatedIndexName)));
        }

        private bool IndexExists<T>() where T : class
        {
            var indexExistsResponse = _elasticClient.IndexExists(x => x.Index<T>());
            return indexExistsResponse.Exists;
        }

        public IndexSettings GetIndexSettings(Type indexType)
        {
            var response = _elasticClient.GetIndexSettings(x => x.Index(indexType));
            return response.IndexSettings;
        }

        public void UpdateIndexSettings(Type indexType, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false)
        {
            var type = indexType;
            _elasticClient.UpdateSettings(x => updateSettingsSelector(x).Index(type));

            if (optimize)
            {
                _elasticClient.Optimize(x => x.Indices(new[] { indexType }).MaxNumSegments(5));
            }
        }

        public IEnumerable<IHit<T>> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searchSelector) where T : class
        {
            const string FirstScrollTimeout = "1s";

            Func<ISearchResponse<T>> searchFunc = () => Search<T>(x => searchSelector(x)
                .SearchType(SearchType.Scan)
                .Scroll(FirstScrollTimeout)
                .Size(_nestSettings.BatchSize));

            return new DelegateEnumerable<IHit<T>>(() => new ScrollEnumerator<T>(searchFunc, _elasticClient, _nestSettings.BatchTimeout));
        }

        private sealed class DelegateEnumerable<THit> : IEnumerable<THit>
        {
            private readonly Func<IEnumerator<THit>> _func;

            public DelegateEnumerable(Func<IEnumerator<THit>> func)
            {
                _func = func;
            }

            public IEnumerator<THit> GetEnumerator()
            {
                return _func();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ScrollEnumerator<TDocument> : IEnumerator<IHit<TDocument>>
            where TDocument : class
        {
            private readonly Func<ISearchResponse<TDocument>> _searchFunc;
            private readonly IElasticClient _elasticClient;
            private readonly string _scrollTimeout;

            private string _scrollId;
            private IEnumerator<IHit<TDocument>> _internalEnumerator;

            public ScrollEnumerator(Func<ISearchResponse<TDocument>> searchFunc, IElasticClient elasticClient, string scrollTimeout)
            {
                _searchFunc = searchFunc;
                _elasticClient = elasticClient;
                _scrollTimeout = scrollTimeout;
            }

            public IHit<TDocument> Current { get { return _internalEnumerator.Current; } }
            object IEnumerator.Current { get { return Current; } }

            public bool MoveNext()
            {
                if (_scrollId == null)
                {
                    var searchResponse = _searchFunc();
                    if (searchResponse.Total <= 0)
                    {
                        return false;
                    }

                    _scrollId = searchResponse.ScrollId;
                }

                if (_internalEnumerator == null || !_internalEnumerator.MoveNext())
                {
                    var response = _elasticClient.Scroll<TDocument>(x => x.Scroll(_scrollTimeout).ScrollId(_scrollId));
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

        public sealed class MultiGetDescriptor2 : MultiGetDescriptor
        {
            // TODO {m.pashuk, 05.08.2014}: message https://github.com/elasticsearch/elasticsearch-net/issues/849
            public MultiGetDescriptor2 SourceEnabled2(bool enabled = true)
            {
                Request.RequestParameters.AddQueryString("_source", enabled);
                return this;
            }

            public MultiGetDescriptor2 GetManyDistinct<T>(IEnumerable<string> ids)
                where T : class
            {
                var documentType = typeof(T);
                var operations = (List<IMultiGetOperation>)((IMultiGetRequest)this).GetOperations;

                foreach (var id in ids)
                {
                    var idRef = id;
                    var idExists = operations.Exists(x => string.Equals(x.Id, idRef, StringComparison.OrdinalIgnoreCase) && x.ClrType == documentType);
                    if (!idExists)
                    {
                        operations.Add(new MultiGetOperationDescriptor<T>().Id(id));
                    }
                }

                return this;
            }
        }
    }
}