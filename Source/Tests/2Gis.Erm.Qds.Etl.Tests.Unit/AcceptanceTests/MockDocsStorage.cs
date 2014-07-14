using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using Moq;

using Nest;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class MockDocsStorage : IDocsStorage, IElasticApi
    {
        private readonly List<Tuple<IDoc, string, object>> _data = new List<Tuple<IDoc, string, object>>();
        readonly List<IDoc> _newPublishedDocs = new List<IDoc>();

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            return from tuple in _data
                   where IsPassesCondition(typeof(TDoc), query, tuple)
                   select (TDoc)tuple.Item1;
        }

        public TDoc GetById<TDoc>(string id) where TDoc : class, IDoc
        {
            return Find<TDoc>(new CursorQueryDsl().ByFieldValue("id", id)).Single();
        }

        public void Update(IEnumerable<IDoc> documents)
        {
            _newPublishedDocs.AddRange(documents);

            TryUpdateRecordIdStateDoc((RecordIdState)_newPublishedDocs.FirstOrDefault(d => d is RecordIdState));
        }

        public void Flush()
        {
        }

        private void TryUpdateRecordIdStateDoc(RecordIdState newState)
        {
            if (newState == null)
                return;

            var oldState = _data.FirstOrDefault(t => t.Item1 is RecordIdState);

            _data.Remove(oldState);
            _data.Add(new Tuple<IDoc, string, object>(newState, oldState.Item2, oldState.Item3));

        }

        public List<IDoc> NewPublishedDocs
        {
            get { return _newPublishedDocs; }
        }

        private bool IsPassesCondition(Type type, IDocsQuery docsQuery, Tuple<IDoc, string, object> tuple)
        {
            var q = (CursorFilterDocsQuey)docsQuery;

            return tuple.Item1.GetType() == type && q.IsChecked(tuple.Item2, tuple.Item3);
        }

        public void Add<TDoc>(TDoc doc, string fieldName, object value) where TDoc : IDoc
        {
            _data.Add(new Tuple<IDoc, string, object>(doc, fieldName, value));
        }

        public void ClearPublishedDocs()
        {
            _newPublishedDocs.Clear();
        }

        public class CursorQueryDsl : IQueryDsl
        {
            public IDocsQuery ByFieldValue(string docFieldName, object docValue)
        {
                return new CursorFilterDocsQuey((path, value) => path.Contains(docFieldName) && docValue.Equals(value));
            }

            public IDocsQuery ByNestedObjectQuery(string nestedObjectName, IDocsQuery nestedQuery)
            {
                return new CursorFilterDocsQuey((path, value) =>
                    {
                        return path.Contains(nestedObjectName) && ((CursorFilterDocsQuey)nestedQuery).IsChecked(path, value);
                    });
            }

            public IDocsQuery Or(IDocsQuery leftQuery, IDocsQuery rightQuery)
            {
                var left = ((CursorFilterDocsQuey)leftQuery);
                var right = ((CursorFilterDocsQuey)rightQuery);

                return new CursorFilterDocsQuey((path, value) => left.IsChecked(path, value) || right.IsChecked(path, value));
            }
        }

        class CursorFilterDocsQuey : IDocsQuery
        {
            readonly Func<string, object, bool> _filter;

            public CursorFilterDocsQuey(Func<string, object, bool> filter)
            {
                _filter = filter;
            }

            public bool IsChecked(string fieldPath, object value)
            {
                return _filter(fieldPath, value);
            }
        }

        # region IElasticApi implementation

        T IElasticApi.Get<T>(string id)
        {
            var document = _data.SingleOrDefault(x => x.Item1 is T && x.Item2 == "Id" && x.Item3.ToString() == id);
            if (document == null)
            {
                return null;
            }

            return (T)document.Item1;
        }

        public ICollection<IMultiGetHit<T>> MultiGet<T>(ICollection<string> ids) where T : class
        {
            var documents = _data.Where(x => x.Item1 is T).Select(x =>
            {
                var mock = new Mock<IMultiGetHit<T>>();
                mock.SetupGet(y => y.Id).Returns(x.Item1.Id);
                mock.SetupGet(y => y.Source).Returns((T)x.Item1);

                return mock.Object;
            }).ToArray();

            return documents;
        }

        public IEnumerable<IHit<T>> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
        {
            var documents = _data.Where(x => x.Item1 is T).Select(x =>
        {
                var mock = new Mock<IHit<T>>();
                mock.SetupGet(y => y.Id).Returns(x.Item1.Id);
                mock.SetupGet(y => y.Source).Returns((T)x.Item1);

                return mock.Object;
            });

            return documents;
        }

        public void Delete<T>(Func<DeleteDescriptor<T>, DeleteDescriptor<T>> deleteSelector) where T : class
        {
            throw new NotImplementedException();
        }

        public void Index(object @object, Type type, string id)
        {
            throw new NotImplementedException();
        }

        public void Index<T>(T @object, Func<IndexDescriptor<T>, IndexDescriptor<T>> indexSelector = null) where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICollection<T>> CreateBatches<T>(IEnumerable<T> items)
        {
            return new[] { items.ToArray() };
        }

        void IElasticApi.Bulk(IEnumerable<Func<Nest.BulkDescriptor, Nest.BulkDescriptor>> bulkDescriptors)
        {
            throw new NotImplementedException();
        }

        public void Bulk(IEnumerable<Func<BulkDescriptor, BulkDescriptor>> selectors)
        {
            throw new NotImplementedException();
        }

        public void Refresh(Func<RefreshDescriptor, RefreshDescriptor> refreshSelector = null)
        {
        }

        # endregion


        public ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
        {
            throw new NotImplementedException();
        }
    }
}