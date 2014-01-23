using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using Newtonsoft.Json;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class ElasticDocsStorageSpecs
    {
        [Subject(typeof(ElasticDocsStorage))]
        class When_find_by_unsupported_query : ElasticDocsStorageContext
        {
            Because of = () => Result = Catch.Exception(() => Target.Find<TestDoc>(Mock.Of<IDocsQuery>()));

            It should_throw_unsupported_exception = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result { get; set; }
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_find_test_docs_by_field_value : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    _expectedDocs = new[] { new TestDoc(), };

                    var mockResponse = new Mock<IQueryResponse<TestDoc>>();
                    mockResponse.SetupGet(r => r.Documents).Returns(_expectedDocs);

                    _query = new FieldValueQuery("fieldName", "field Value");

                    Elastic.Setup(e => e.Search<TestDoc>(Moq.It.IsAny<Func<SearchDescriptor<TestDoc>, SearchDescriptor<TestDoc>>>()))
                           .Returns(mockResponse.Object)
                           .Callback<Func<SearchDescriptor<TestDoc>, SearchDescriptor<TestDoc>>>(sd => _searchDescr = sd);
                };

            Because of = () => _result = Target.Find<TestDoc>(_query);

            It should_return_expected_docs = () => _result.Should().BeEquivalentTo(_expectedDocs);

            // TODO Кривой тест, надо подумать как поправить, если это возможно
            It shold_request_docs_by_field_value = () =>
                {
                    var descriptor = _searchDescr(new SearchDescriptor<TestDoc>());
                    var json = JsonConvert.SerializeObject(descriptor);

                    json.Should().Contain(_query.FieldName);
                    json.Should().Contain(_query.FieldValue.ToString());
                };

            static FieldValueQuery _query;
            static IEnumerable<TestDoc> _result;
            static TestDoc[] _expectedDocs;
            static Func<SearchDescriptor<TestDoc>, SearchDescriptor<TestDoc>> _searchDescr;
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_update_docs : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    _expected = new[] { new TestDoc() };

                    Elastic.Setup(e => e.IndexMany(_expected)).Callback(() => _actual = _expected);
                };

            Because of = () => Target.Update(_expected);

            It should_index_many_passed_docs = () => _actual.Should().BeEquivalentTo(_expected);

            static IEnumerable<IDoc> _actual;
            static IEnumerable<IDoc> _expected;
        }

        class ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    Elastic = new Mock<IElasticClient>();
                    Target = new ElasticDocsStorage(Elastic.Object);
                };

            protected static Mock<IElasticClient> Elastic { get; private set; }

            protected static ElasticDocsStorage Target { get; private set; }
        }
    }

    public class TestDoc : IDoc
    {
    }
}
