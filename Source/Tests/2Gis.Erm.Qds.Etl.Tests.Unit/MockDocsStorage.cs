using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit
{
    class MockDocsStorage : IDocsStorage
    {
        private readonly List<Tuple<IDoc, string, object>> _data = new List<Tuple<IDoc, string, object>>();

        public MockDocsStorage()
        {
            PublishedDocs = new IDoc[0];
        }

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            return from tuple in _data
                   where IsPassesCondition(typeof(TDoc), query, tuple)
                   select (TDoc)tuple.Item1;
        }

        public void Update(IEnumerable<IDoc> docs)
        {
            var x = docs.ToArray();
            var y = PublishedDocs.ToArray();

            var z = new IDoc[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);

            PublishedDocs = z;
        }

        public IDoc[] PublishedDocs { get; private set; }

        private bool IsPassesCondition(Type type, IDocsQuery docsQuery, Tuple<IDoc, string, object> tuple)
        {
            var q = (DocsQuery)docsQuery;

            return tuple.Item1.GetType() == type && q.DocFieldName == tuple.Item2 && tuple.Item3.Equals(q.Value);
        }

        public void Add<TDoc>(TDoc doc, string fieldName, object value) where TDoc : IDoc
        {
            _data.Add(new Tuple<IDoc, string, object>(doc, fieldName, value));
        }

        public class QueryDsl : IQueryDsl
        {
            public IDocsQuery ByFieldValue(string docFieldName, object value)
            {
                return new DocsQuery(docFieldName, value);
            }
        }

        class DocsQuery : IDocsQuery
        {
            public string DocFieldName { get; set; }
            public object Value { get; set; }

            public DocsQuery(string docFieldName, object value)
            {
                DocFieldName = docFieldName;
                Value = value;
            }
        }
    }
}