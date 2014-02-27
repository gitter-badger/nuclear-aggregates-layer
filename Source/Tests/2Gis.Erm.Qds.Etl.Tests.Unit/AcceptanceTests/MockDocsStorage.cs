using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Etl.Extract.EF;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class MockDocsStorage : IDocsStorage
    {
        private readonly List<Tuple<IDoc, string, object>> _data = new List<Tuple<IDoc, string, object>>();
        readonly List<IDoc> _newPublishedDocs = new List<IDoc>();

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            return from tuple in _data
                   where IsPassesCondition(typeof(TDoc), query, tuple)
                   select (TDoc)tuple.Item1;
        }

        public void Update(IEnumerable<IDoc> documents)
        {
            var docs = documents.ToArray();

            TryUpdateRecordIdStateDoc((RecordIdState)docs.FirstOrDefault(d => d is RecordIdState));
            _newPublishedDocs.AddRange(docs);
        }

        private void TryUpdateRecordIdStateDoc(RecordIdState newState)
        {
            if (newState == null)
                return;

            var oldState = _data.FirstOrDefault(t => t.Item1 is RecordIdState);

            _data.Remove(oldState);
            _data.Add(new Tuple<IDoc, string, object>(newState, oldState.Item2, oldState.Item3));

        }

        public IDoc[] NewPublishedDocs
        {
            get { return _newPublishedDocs.ToArray(); }
        }

        private bool IsPassesCondition(Type type, IDocsQuery docsQuery, Tuple<IDoc, string, object> tuple)
        {
            var q = (DocsQuery)docsQuery;

            return tuple.Item1.GetType() == type && q.DocFieldName == tuple.Item2 && tuple.Item3.Equals(q.Value);
        }

        public void Add<TDoc>(TDoc doc, string fieldName, object value) where TDoc : IDoc
        {
            _data.Add(new Tuple<IDoc, string, object>(doc, fieldName, value));
        }

        public void ClearPublishedDocs()
        {
            _newPublishedDocs.Clear();
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
            public string DocFieldName { get; private set; }
            public object Value { get; private set; }

            public DocsQuery(string docFieldName, object value)
            {
                DocFieldName = docFieldName;
                Value = value;
            }
        }
    }
}