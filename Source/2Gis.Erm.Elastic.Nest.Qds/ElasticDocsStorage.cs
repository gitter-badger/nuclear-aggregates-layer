using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticDocsStorage : IDocsStorage
    {
        readonly IElasticClient _elastic;

        public ElasticDocsStorage(IElasticClient elastic)
        {
            if (elastic == null)
            {
                throw new ArgumentNullException("elastic");
            }

            _elastic = elastic;
        }

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            // TODO При развитии функциональности запросов, надо перенести функциональность формирования запроса в сам IDocsQuery, тут оставить только вызов
            var fieldQuery = query as FieldValueQuery;
            if (fieldQuery == null)
                throw new NotSupportedException(query.GetType().FullName);

            var queryDescriptor = new QueryDescriptor();
            queryDescriptor.Term(fieldQuery.FieldName, fieldQuery.FieldValue);

            var searchDescr = new SearchDescriptor<TDoc>();

            searchDescr.Query(queryDescriptor);

            var queryResponse = _elastic.Search<TDoc>(sd => sd.Query(qd => qd.Term(fieldQuery.FieldName, fieldQuery.FieldValue)));

            return queryResponse.Documents;
        }

        public void Update(IEnumerable<IDoc> docs)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            _elastic.IndexMany(docs);
        }
    }
}
