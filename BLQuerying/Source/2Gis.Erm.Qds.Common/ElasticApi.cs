using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using DoubleGis.Erm.Qds.Common.Settings;
using Elasticsearch.Net;
using Elasticsearch.Net.Connection;
using Elasticsearch.Net.Connection.Thrift;
using Elasticsearch.Net.Connection.Thrift.Protocol;
using Elasticsearch.Net.Serialization;

using Nest;
using Nest.Resolvers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DoubleGis.Erm.Qds.Common
{
    public enum UpdateType
    {
        UpdateOverride,
        UpdateMerge,
    }

    public sealed class ElasticApi : IElasticApi, IElasticManagementApi
    {
        private readonly IElasticClient _elasticClient;
        private readonly INestSettings _settings;

        public ElasticApi(INestSettings settings, IConnectionSettingsValues connectionSettings)
        {
            _settings = settings;

            var serializer = new InternalSerializer(connectionSettings);
            _elasticClient = new ElasticClient(connectionSettings, CreateConnection(settings, connectionSettings), serializer);
        }

        public bool TypeExists<T>() where T : class
        {
            var response = _elasticClient.TypeExists(x => x.Index<T>().Type<T>());
            return response.Exists;
        }

        public void Bulk(IReadOnlyCollection<Func<ErmBulkDescriptor, ErmBulkDescriptor>> funcs)
        {
            Func<ErmBulkDescriptor, ErmBulkDescriptor> aggregatedFunc = x => funcs.Aggregate(x, (current, func) => func(current));
            var bulkResponse = _elasticClient.Bulk(x => aggregatedFunc(new ErmBulkDescriptor()));
            if (!bulkResponse.Errors)
            {
                return;
            }

            var firstError = bulkResponse.ItemsWithErrors.First().Error;
            throw new ApplicationException(firstError);
        }

        public IEnumerable<IReadOnlyCollection<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            var buffer = new List<T>(_settings.BatchSize);

            foreach (var item in items)
            {
                buffer.Add(item);

                if (buffer.Count == buffer.Capacity)
                {
                    yield return buffer;
                    buffer = new List<T>(_settings.BatchSize);
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

        public IDocumentWrapper<T> Create<T>(T @object, string id = null) where T : class
        {
            var response = _elasticClient.Index(@object,
            x =>
            {
                    if (id != null)
                    {
                        x = x.Id(id);
                    }

                return x.OpType(OpType.Create);
            });

            return new DocumentWrapper<T>
            {
                Id  = response.Id,
                Document = @object,
                Version = long.Parse(response.Version),
            };
        }

        public IDocumentWrapper<T> Update<T>(IDocumentWrapper<T> documentWrapper) where T : class
        {
            var response = _elasticClient.Update<T, T>(x =>
            {
                if (documentWrapper.Version != null)
                {
                    x = x.Version(documentWrapper.Version.Value);
                }

                return x.Id(documentWrapper.Id).Doc(documentWrapper.Document);
            });

            return new DocumentWrapper<T>
            {
                Id = response.Id,
                Document = documentWrapper.Document,
                Version = long.Parse(response.Version),
            };
        }

        public IDocumentWrapper<T> Delete<T>(IDocumentWrapper<T> documentWrapper) where T : class
        {
            var response = _elasticClient.Delete<T>(x =>
            {
                if (documentWrapper.Version != null)
                {
                    x = x.Version(documentWrapper.Version.Value);
                }

                return x.Id(documentWrapper.Id);
            });

            return new DocumentWrapper<T>
            {
                Id = response.Id,
                Document = documentWrapper.Document,
                Version = long.Parse(response.Version),
            };
        }

        public T Get<T>(string id) where T : class
        {
            var response = _elasticClient.Get<T>(x => x.Id(id));
            return response.Source;
        }

        public IReadOnlyCollection<IMultiGetHit<object>> MultiGet(Func<ErmMultiGetDescriptor, ErmMultiGetDescriptor> multiGetSelector)
        {
            var response = _elasticClient.MultiGet(x => multiGetSelector(new ErmMultiGetDescriptor()));
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
            var isolatedIndexName = _settings.IndexPrefix + "." + alias.ToLowerInvariant();
            _elasticClient.Alias(x => x.Add(y => y.Index<T>().Alias(isolatedIndexName)));
        }

        public IndexSettings GetIndexSettings(Type indexType)
        {
            var response = _elasticClient.GetIndexSettings(x => x.Index(indexType));
            return response.IndexSettings;
        }

        public void UpdateIndexSettings(Type indexType, Func<UpdateSettingsDescriptor, UpdateSettingsDescriptor> updateSettingsSelector, bool optimize = false)
        {
            if (optimize)
            {
                _elasticClient.Optimize(x => x.Indices(indexType));
            }

            _elasticClient.UpdateSettings(x => updateSettingsSelector(x).Index(indexType));
        }

        public IEnumerable<IDocumentWrapper<T>> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searchSelector, IProgress<long> progress = null) where T : class
        {
            const string FirstScrollTimeout = "1s";

            Func<ISearchResponse<T>> searchFunc = () => Search<T>(x => searchSelector(x)
                .SearchType(SearchType.Scan)
                .Scroll(FirstScrollTimeout)
                .Size(_settings.BatchSize));

            return new DelegateEnumerable<IDocumentWrapper<T>>(() => new ScrollEnumerator<T>(_elasticClient, searchFunc, _settings.BatchTimeout, progress));
        }

        private static IConnection CreateConnection(INestSettings settings, IConnectionSettingsValues connectionSettings)
        {
            switch (settings.Protocol)
            {
                case Protocol.Http:
                    return new HttpClientConnection(connectionSettings,
                                                    new WebRequestHandler
                                                    {
                                                        UseDefaultCredentials = true,
                                                        PreAuthenticate = true,
                                                    });
                case Protocol.Thrift:
                    return new ThriftConnection(connectionSettings, new TBinaryProtocol.Factory());
                case Protocol.ThriftCompact:
                    return new ThriftConnection(connectionSettings, new TCompactProtocol.Factory());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IndexExists<T>() where T : class
        {
            var indexExistsResponse = _elasticClient.IndexExists(x => x.Index<T>());
            return indexExistsResponse.Exists;
        }

        public sealed class ErmBulkDescriptor : BulkDescriptor
        {
            public ErmBulkDescriptor Update<T>(Func<BulkUpdateDescriptor<T, T>, BulkUpdateDescriptor<T, T>> bulkUpdateSelector, UpdateType updateType)
                where T : class
            {
                var operations = ((IBulkRequest)this).Operations;
                var newOperation = (IBulkUpdateOperation<T, T>)bulkUpdateSelector(new ErmBulkUpdateDescriptor<T, T> { UpdateType = updateType });

                switch (updateType)
                {
                    case UpdateType.UpdateOverride:
                    {
                        operations.Add(newOperation);
                        break;
                    }

                    case UpdateType.UpdateMerge:
                    {
                        var existingOperation = (IBulkUpdateOperation<T, T>)operations.SingleOrDefault(x => string.Equals(x.Id, newOperation.Id, StringComparison.OrdinalIgnoreCase) && x.ClrType == newOperation.ClrType);
                        if (existingOperation != null)
                        {
                            var existingDoc = JObject.FromObject(existingOperation.Doc);
                            var newDoc = JObject.FromObject(newOperation.Doc);
                            existingDoc.Merge(newDoc);
                            existingOperation.Doc = existingDoc.ToObject<T>();
                        }
                        else
                        {
                            operations.Add(newOperation);
                        }

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException("updateType");
                }

                return this;
            }
        }

        public sealed class ErmMultiGetDescriptor : MultiGetDescriptor
        {
            public ErmMultiGetDescriptor GetDistinct<T>(Func<MultiGetOperationDescriptor<T>, MultiGetOperationDescriptor<T>> getSelector) where T : class
            {
                var descriptor = (IMultiGetOperation)getSelector(new MultiGetOperationDescriptor<T>());

                var documentType = typeof(T);
                var operations = ((IMultiGetRequest)this).GetOperations;
                var idExists = operations.Any(x => string.Equals(x.Id, descriptor.Id, StringComparison.OrdinalIgnoreCase) && x.ClrType == documentType);
                if (!idExists)
                {
                    operations.Add(descriptor);
                }

                return this;
            }
        }

        private sealed class InternalSerializer : NestSerializer
        {
            private readonly IConnectionSettingsValues _settings;

            public InternalSerializer(IConnectionSettingsValues settings)
                : base(settings)
            {
                _settings = settings;
            }

            public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented)
            {
                var ermBulkOperationBody = data as ErmBulkOperationBody;
                if (ermBulkOperationBody == null)
                {
                    return base.Serialize(data, formatting);
                }

                var formatting2 = (formatting == SerializationFormatting.None) ? Formatting.None : Formatting.Indented;
                var settings = new JsonSerializerSettings { ContractResolver = new ElasticContractResolver(_settings) };

                switch (ermBulkOperationBody.UpdateType)
                {
                    case UpdateType.UpdateOverride:
                        settings.DefaultValueHandling = DefaultValueHandling.Include;
                        settings.NullValueHandling = NullValueHandling.Include;
                        break;
                    case UpdateType.UpdateMerge:
                        settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, formatting2, settings));
            }
        }

        private sealed class ErmBulkUpdateDescriptor<TDocument, TPartialDocument> : BulkUpdateDescriptor<TDocument, TPartialDocument>
            where TDocument : class
            where TPartialDocument : class
        {
            public UpdateType UpdateType { private get; set; }

            protected override object GetBulkOperationBody()
            {
                var self = (IBulkUpdateOperation<TDocument, TPartialDocument>)this;
                return new ErmBulkOperationBody { Doc = self.Doc, UpdateType = UpdateType };
            }
        }

        private sealed class ErmBulkOperationBody
        {
            [JsonProperty(PropertyName = "doc")]
            public object Doc { get; set; }

            [JsonIgnore]
            public UpdateType UpdateType { get; set; }
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

        private sealed class ScrollEnumerator<TDocument> : IEnumerator<IDocumentWrapper<TDocument>>
            where TDocument : class
        {
            private readonly IElasticClient _elasticClient;
            private readonly Func<ISearchResponse<TDocument>> _searchFunc;
            private readonly string _scrollTimeout;
            private readonly IProgress<long> _progress;

            private string _scrollId;
            private IEnumerator<IHit<TDocument>> _internalEnumerator;

            public ScrollEnumerator(IElasticClient elasticClient, Func<ISearchResponse<TDocument>> searchFunc, string scrollTimeout, IProgress<long> progress = null)
            {
                _elasticClient = elasticClient;
                _searchFunc = searchFunc;
                _scrollTimeout = scrollTimeout;
                _progress = progress;
            }

            public IDocumentWrapper<TDocument> Current
            {
                get
                {
                    var current = _internalEnumerator.Current;
                    return new DocumentWrapper<TDocument>
                    {
                        Id = current.Id,
                        Document = current.Source,
                        Version = current.Version == null ? (long?)null : long.Parse(current.Version),
                    };
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_scrollId == null)
                {
                    var searchResponse = _searchFunc();

                    if (_progress != null)
                    {
                        _progress.Report(searchResponse.Total);
                    }

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
    }
}