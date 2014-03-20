using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticDocsStorage : IDocsStorage
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly IElasticMeta _elasticMeta;
        private readonly IElasticResponseHandler _responseHandler;

        public ElasticDocsStorage(IElasticClientFactory elasticClientFactory, IElasticMeta elasticMeta, IElasticResponseHandler responseHandler)
        {
            if (elasticClientFactory == null)
            {
                throw new ArgumentNullException("elasticClientFactory");
            }
            if (elasticMeta == null)
            {
                throw new ArgumentNullException("elasticMeta");
            }
            if (responseHandler == null)
            {
                throw new ArgumentNullException("responseHandler");
            }

            _elasticClientFactory = elasticClientFactory;
            _elasticMeta = elasticMeta;
            _responseHandler = responseHandler;
        }

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var searchDescriptor = _elasticMeta.CreatePage<TDoc>(query);

            while (searchDescriptor != null)
            {
                var sd = searchDescriptor;
                var response = _elasticClientFactory.UsingElasticClient(ec => ec.Search(sd.SearchDescriptor));
                _responseHandler.ThrowWhenError(response);

                foreach (var doc in response.Documents)
                {
                    yield return doc;
                }

                searchDescriptor = _elasticMeta.NextPage(searchDescriptor, response);
            }
        }

        /// <summary>
        /// Данная реализация отправляет документы по одному, что очень чувствительно с точки зрения удаленных вызовов. 
        /// Нужно его заменить на реализацию в UpdateWithBulkDraft
        /// </summary>
        public void Update(IEnumerable<IDoc> docs)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            foreach (var doc in docs)
            {
                // TODO ERM-3449 Группировать по типу и вызывать Bulk

                IDoc d = doc;
                var type = d.GetType();

                var indexResponse = _elasticClientFactory.UsingElasticClient(ec => ec.Index(d, _elasticMeta.GetIndexName(type), _elasticMeta.GetTypeName(type)));
                _responseHandler.ThrowWhenError(indexResponse);
            }

            // TODO ERM-3449 Вызывать рефреш индекса только если были изменения + Test не забыть
            var shardResponse = _elasticClientFactory.UsingElasticClient(ec => ec.Refresh());
            _responseHandler.ThrowWhenError(shardResponse);
        }

        /// <summary>
        /// Надо заменить Update на эту реализацию, написав тесты.
        /// Данная реализация использует рефлекшин, чтобы вызвать Bulk индексацию по типу документа.
        /// Данный подход обладает ограничением в районе 100к документов, падает.
        /// </summary>
        public void UpdateWithBulkDraft(IEnumerable<IDoc> docs)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            var docsArray = docs.ToArray();
            if (docsArray.Length == 0)
                return;

            var descr = new BulkDescriptor();
            Type docType = null;
            MethodInfo mi = null;

            foreach (var doc in docsArray)
            {
                var curDocType = doc.GetType();
                if (docType != curDocType)
                {
                    docType = curDocType;
                    var method = typeof(ElasticDocsStorage).GetMethod("DescrIndex", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi = method.MakeGenericMethod(docType);
                }

                mi.Invoke(this, new object[] { descr, doc });
            }

            var response = _elasticClientFactory.UsingElasticClient(ec => ec.Bulk(descr));
            _responseHandler.ThrowWhenError(response);

            var shardResponse = _elasticClientFactory.UsingElasticClient(ec => ec.Refresh());
            _responseHandler.ThrowWhenError(shardResponse);
        }

        void DescrIndex<T>(BulkDescriptor descr, T doc) where T : class
        {
            var type = typeof(T);

            descr.Index<T>(d => d.Object(doc)
                                    .Index(_elasticMeta.GetIndexName(type))
                                    .Type(_elasticMeta.GetTypeName(type)));
        }

    }
}
