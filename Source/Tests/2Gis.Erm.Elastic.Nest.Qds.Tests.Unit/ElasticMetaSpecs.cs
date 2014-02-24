using System.Collections.Generic;

using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    // TODO Привести тесты в порядок
    class ElasticMetaSpecs
    {
        [Subject(typeof(ElasticMeta))]
        class When_next_page_for_last_response : ElasticMetaContext
        {
            Establish context = () =>
            {
                PageDescriptor = new PagedSearchDescriptor<TestDoc>(new SearchDescriptor<TestDoc>(), 42, ElasticMeta.PageSize);

                var mockResponse = new Mock<IQueryResponse<TestDoc>>();
                mockResponse.SetupGet(r => r.Total).Returns(PageDescriptor.From + PageDescriptor.Size);

                Response = mockResponse.Object;
            };

            Because of = () => Result = Target.NextPage(PageDescriptor, Response);

            It should_return_null = () => Result.Should().BeNull();

            static PagedSearchDescriptor<TestDoc> Result;
            static IQueryResponse<TestDoc> Response;
            static PagedSearchDescriptor<TestDoc> PageDescriptor;
        }

        [Subject(typeof(ElasticMeta))]
        class When_next_page_for_not_last_response : ElasticMetaContext
        {
            Establish context = () =>
                {
                    PageDescriptor = new PagedSearchDescriptor<TestDoc>(new SearchDescriptor<TestDoc>(), 42, ElasticMeta.PageSize);
                    _expectedFrom = PageDescriptor.From + ElasticMeta.PageSize;

                    var mockResponse = new Mock<IQueryResponse<TestDoc>>();
                    mockResponse.SetupGet(r => r.Total).Returns(PageDescriptor.From + PageDescriptor.Size + 1);
                    Response = mockResponse.Object;
                };

            Because of = () => Result = Target.NextPage(PageDescriptor, Response);

            It should_set_from = () => Result.From.Should().Be(_expectedFrom);
            It should_set_size = () => Result.Size.Should().Be(ElasticMeta.PageSize);

            static PagedSearchDescriptor<TestDoc> Result;
            static IQueryResponse<TestDoc> Response;
            static PagedSearchDescriptor<TestDoc> PageDescriptor;
            static int _expectedFrom;
        }

        [Subject(typeof(ElasticMeta))]
        class When_create_page : ElasticMetaContext
        {
            Establish context = () => SettingsFactory.Setup(sf => sf.GetIsolatedIndexName(Moq.It.IsAny<string>())).Returns("some.Index");

            Because of = () => Result = Target.CreatePage<TestDoc>(new NestQueryDsl().ByFieldValue("Id", 0));

            It should_init_search_descriptor = () => Result.SearchDescriptor.Should().NotBeNull();
            It should_set_from = () => Result.From.Should().Be(0);
            It should_set_size = () => Result.Size.Should().Be(ElasticMeta.PageSize);

            static PagedSearchDescriptor<TestDoc> Result;
        }

        [Subject(typeof(ElasticMeta))]
        class When_get_index_name_by_data_type_document : ElasticMetaContext
        {
            Establish context = () =>
                {
                    DocType = typeof(ClientGridDoc).Name;
                    ExpectedIndexName = "expected.index.name";

                    SettingsFactory.Setup(sf => sf.GetIsolatedIndexName(ElasticMeta.DataIndexName)).Returns(ExpectedIndexName);
                };

            Because of = () => Result = Target.GetIndexName(DocType);

            It should_request_isolated_index_for_document_type = () => Result.Should().Be(ExpectedIndexName);

            static string ExpectedIndexName;
            static string Result;
            static string DocType;
        }

        [Subject(typeof(ElasticMeta))]
        class When_get_index_name_by_catalog_type_document : ElasticMetaContext
        {
            Establish context = () =>
            {
                DocType = "CatalogTypeDoc";
                ExpectedIndexName = "expected.index.name";

                SettingsFactory.Setup(sf => sf.GetIsolatedIndexName(ElasticMeta.CatalogIndexName)).Returns(ExpectedIndexName);
            };

            Because of = () => Result = Target.GetIndexName(DocType);

            It should_request_isolated_index_for_document_type = () => Result.Should().Be(ExpectedIndexName);

            static string ExpectedIndexName;
            static string Result;
            static string DocType;
        }

        class ElasticMetaContext
        {
            Establish context = () =>
                {
                    SettingsFactory = new Mock<IElasticConnectionSettingsFactory>();
                    Target = new ElasticMeta(SettingsFactory.Object);
                };

            protected static Mock<IElasticConnectionSettingsFactory> SettingsFactory { get; private set; }
            protected static ElasticMeta Target { get; private set; }
        }
    }
}