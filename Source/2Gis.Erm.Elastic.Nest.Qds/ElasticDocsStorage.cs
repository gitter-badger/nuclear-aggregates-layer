using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticDocsStorage : IDocsStorage
    {
        private readonly IElasticApi _elasticApi;
        private readonly ISearchDescriptorPaging _searchDescriptorPaging;

        public ElasticDocsStorage(IElasticApi elasticApi, ISearchDescriptorPaging searchDescriptorPaging)
        {
            if (elasticApi == null)
            {
                throw new ArgumentNullException("elasticApi");
            }
            if (searchDescriptorPaging == null)
            {
                throw new ArgumentNullException("searchDescriptorPaging");
            }

            _elasticApi = elasticApi;
            _searchDescriptorPaging = searchDescriptorPaging;
        }

        // FIXME {f.zaharov, 11.04.2014}: надо заменить на использование scroll
        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var searchDescriptor = _searchDescriptorPaging.CreatePage<TDoc>(query);

            while (searchDescriptor != null)
            {
                var response = _elasticApi.Search(searchDescriptor.SearchDescriptor);

                foreach (var doc in response.Documents)
                {
                    yield return doc;
                }

                searchDescriptor = _searchDescriptorPaging.NextPage(searchDescriptor, response);
            }
        }

        public TDoc GetById<TDoc>(string id) where TDoc : class, IDoc
        {
            var document = _elasticApi.MultiGet<TDoc>(new[] { id }).Where(x => x.Found).Select(x => x.Source).FirstOrDefault();
            return document;
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
                var documentType = d.GetType();

                _elasticApi.Index(d, documentType, d.Id);
            }
        }

        /// <summary>
        /// Надо заменить Update на эту реализацию, написав тесты.
        /// Данная реализация использует рефлекшин, чтобы вызвать Bulk индексацию по типу документа.
        /// Данный подход обладает ограничением в районе 100к документов, падает.
        /// </summary>
        private void UpdateWithBulkDraft(IEnumerable<IDoc> docs)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            var docsArray = docs.ToArray();
            if (docsArray.Length == 0)
                return;

            Func<BulkDescriptor, BulkDescriptor> func = x =>
            {
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

                    mi.Invoke(this, new object[] { x, doc });
                }
                return x;
            };

            _elasticApi.Bulk(new[] { func });
        }

        void DescrIndex<T>(BulkDescriptor descr, T doc) where T : class
        {
            descr.Index<T>(d => d.Object(doc));
        }

        public void Flush()
        {
            _elasticApi.Refresh();
        }
    }
}
