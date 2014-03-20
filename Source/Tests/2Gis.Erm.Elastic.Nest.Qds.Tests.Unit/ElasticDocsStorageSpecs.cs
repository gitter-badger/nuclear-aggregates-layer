using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class ElasticDocsStorageSpecs
    {
        [Subject(typeof(ElasticDocsStorage))]
        class When_find : ElasticDocsStorageContext
        {
            Establish context = () =>
            {
                Query = Mock.Of<IDocsQuery>();
                FirstExpectedDoc = new TestDoc();
                SecondExpectedDoc = new TestDoc();

                IQueryResponse<TestDoc> firstResponse;
                var firstPageDescriptor = SetupPageSearchDescriptor(FirstExpectedDoc, out firstResponse);

                IQueryResponse<TestDoc> secondResponse;
                var secondPageDescriptor = SetupPageSearchDescriptor(SecondExpectedDoc, out secondResponse);

                ElasticMeta.Setup(em => em.CreatePage<TestDoc>(Query)).Returns(firstPageDescriptor);
                ElasticMeta.Setup(em => em.NextPage(firstPageDescriptor, firstResponse)).Returns(secondPageDescriptor);
                ElasticMeta.Setup(em => em.NextPage(secondPageDescriptor, secondResponse)).Returns<PagedSearchDescriptor<TestDoc>>(null);
            };

            Because of = () => Result = Target.Find<TestDoc>(Query);

            It should_return_docs_for_each_result = () => Result.Should().BeEquivalentTo(new object[] { FirstExpectedDoc, SecondExpectedDoc });

            static PagedSearchDescriptor<TestDoc> SetupPageSearchDescriptor(TestDoc expectedDoc, out IQueryResponse<TestDoc> queryResponse)
            {
                var mockQueryResponse = new Mock<IQueryResponse<TestDoc>>();
                mockQueryResponse.SetupGet(q => q.Documents).Returns(new[] { expectedDoc });
                queryResponse = mockQueryResponse.Object;

                var searchDescriptor = new PagedSearchDescriptor<TestDoc>(new SearchDescriptor<TestDoc>(), 0, 40);
                ElasticClient.Setup(e => e.Search(searchDescriptor.SearchDescriptor)).Returns(queryResponse);

                return searchDescriptor;
            }

            static IEnumerable<TestDoc> Result;
            static TestDoc FirstExpectedDoc;
            static TestDoc SecondExpectedDoc;
            static IDocsQuery Query;
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_find_and_serach_response_is_not_valid : ElasticDocsStorageContext
        {
            Establish context = () =>
            {
                var invalidResponse = Mock.Of<IQueryResponse<TestDoc>>();
                ResponseHandler.Setup(h => h.ThrowWhenError(invalidResponse)).Throws<ElasticException>();

                ElasticClient.Setup(e => e.Search(Moq.It.IsAny<SearchDescriptor<TestDoc>>())).Returns(invalidResponse);

                ElasticMeta.Setup(m => m.CreatePage<TestDoc>(Moq.It.IsAny<IDocsQuery>()))
                    .Returns(new PagedSearchDescriptor<TestDoc>(new SearchDescriptor<TestDoc>(), 0, 40));
            };

            Because of = () => Result = Catch.Exception(() => Target.Find<TestDoc>(Mock.Of<IDocsQuery>()).ToArray());

            It should_throw_docs_storage_exception = () => Result.Should().NotBeNull().And.BeOfType<ElasticException>();

            static Exception Result;
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_update_and_index_response_is_not_valid : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    TestDoc = Mock.Of<IDoc>();
                    var invalidResponse = Mock.Of<IndexResponse>();
                    ElasticClient.Setup(e => e.Index(TestDoc, Moq.It.IsAny<string>(), Moq.It.IsAny<string>())).Returns(invalidResponse);

                    ResponseHandler.Setup(h => h.ThrowWhenError(invalidResponse)).Throws<ElasticException>();
                };

            Because of = () => Result = Catch.Exception(() => Target.Update(new[] { TestDoc }));

            It should_throw_docs_storage_exception = () => Result.Should().NotBeNull().And.BeOfType<ElasticException>();

            static Exception Result;
            static IDoc TestDoc;
        }

        class ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ElasticClient = new Mock<IElasticClient>();
                    ElasticMeta = new Mock<IElasticMeta>();
                    ResponseHandler = new Mock<IElasticResponseHandler>();

                    Target = new ElasticDocsStorage(new MockElasticClientFactory(ElasticClient.Object), ElasticMeta.Object, ResponseHandler.Object);
                };

            protected static Mock<IElasticResponseHandler> ResponseHandler { get; private set; }
            protected static Mock<IElasticClient> ElasticClient { get; private set; }
            protected static Mock<IElasticMeta> ElasticMeta { get; private set; }
            protected static ElasticDocsStorage Target { get; private set; }
        }
    }
}
