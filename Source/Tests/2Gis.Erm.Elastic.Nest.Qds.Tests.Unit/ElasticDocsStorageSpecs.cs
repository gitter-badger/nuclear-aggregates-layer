using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common;

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
        class When_get_by_id : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ExpectedDoc = new TestDoc { Id = "42" };
                    ElasticApi.Setup(api => api.Get<TestDoc>("42")).Returns(ExpectedDoc);
                };

            Because of = () => Result = Target.GetById<TestDoc>("42");

            It should_refresh_elastic_api = () => Result.Should().Be(ExpectedDoc);

            static TestDoc ExpectedDoc;
            static TestDoc Result;
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_flush : ElasticDocsStorageContext
        {
            Because of = () => Target.Flush();

            It should_refresh_elastic_api = () => ElasticApi.Verify(es => es.Refresh(null), Times.Once);
        }

        [Subject(typeof(ElasticDocsStorage))]
        class When_find : ElasticDocsStorageContext
        {
            Establish context = () =>
            {
                Query = Mock.Of<IDocsQuery>();
                FirstExpectedDoc = new TestDoc();
                SecondExpectedDoc = new TestDoc();

                ISearchResponse<TestDoc> firstResponse;
                var firstPageDescriptor = SetupPageSearchDescriptor(FirstExpectedDoc, out firstResponse);

                ISearchResponse<TestDoc> secondResponse;
                var secondPageDescriptor = SetupPageSearchDescriptor(SecondExpectedDoc, out secondResponse);

                ElasticMeta.Setup(em => em.CreatePage<TestDoc>(Query)).Returns(firstPageDescriptor);

                ElasticMeta.Setup(em => em.NextPage(firstPageDescriptor, firstResponse)).Returns(secondPageDescriptor);
                ElasticMeta.Setup(em => em.NextPage(secondPageDescriptor, secondResponse)).Returns<PagedSearchDescriptor<TestDoc>>(null);
            };

            Because of = () => Result = Target.Find<TestDoc>(Query);

            It should_return_docs_for_each_result = () => Result.Should().BeEquivalentTo(new object[] { FirstExpectedDoc, SecondExpectedDoc });

            static PagedSearchDescriptor<TestDoc> SetupPageSearchDescriptor(TestDoc expectedDoc, out ISearchResponse<TestDoc> queryResponse)
            {
                var mockQueryResponse = new Mock<ISearchResponse<TestDoc>>();
                mockQueryResponse.SetupGet(q => q.Documents).Returns(new[] { expectedDoc });
                queryResponse = mockQueryResponse.Object;

                var sd = new SearchDescriptor<TestDoc>();

                var pagedSearchDescriptor = new PagedSearchDescriptor<TestDoc>(x => sd, 0, 42);
                ElasticApi.Setup(e => e.Search(Moq.It.Is<Func<SearchDescriptor<TestDoc>, SearchDescriptor<TestDoc>>>(f => f(null) == sd))).Returns(queryResponse);

                return pagedSearchDescriptor;
            }

            static IEnumerable<TestDoc> Result;
            static TestDoc FirstExpectedDoc;
            static TestDoc SecondExpectedDoc;
            static IDocsQuery Query;
        }

        class ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ElasticApi = new Mock<IElasticApi>();
                    ElasticMeta = new Mock<ISearchDescriptorPaging>();

                    Target = new ElasticDocsStorage(ElasticApi.Object, ElasticMeta.Object);
                };

            protected static Mock<IElasticApi> ElasticApi { get; private set; }
            protected static Mock<ISearchDescriptorPaging> ElasticMeta { get; private set; }
            protected static ElasticDocsStorage Target { get; private set; }
        }
    }
}
