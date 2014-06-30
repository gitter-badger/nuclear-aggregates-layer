using System;

using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    // TODO Привести тесты в порядок
    class SearchDescriptorPagingSpecs
    {
        [Subject(typeof(SearchDescriptorPaging))]
        class When_create_page_for_not_supported_query : SearchDescriptorPagingContext
        {
            Because of = () => Result = Catch.Exception(() => Target.CreatePage<TestDoc>(Mock.Of<IDocsQuery>()));

            It should_throw_not_supported_exception = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
        }

        [Subject(typeof(SearchDescriptorPaging))]
        class When_next_page_for_last_response : SearchDescriptorPagingContext
        {
            Establish context = () =>
            {
                PageDescriptor = new PagedSearchDescriptor<TestDoc>(x => x, 42, SearchDescriptorPaging.PageSize);

                var mockResponse = new Mock<ISearchResponse<TestDoc>>();
                mockResponse.SetupGet(r => r.Total).Returns(PageDescriptor.From + PageDescriptor.Size);

                Response = mockResponse.Object;
            };

            Because of = () => Result = Target.NextPage(PageDescriptor, Response);

            It should_return_null = () => Result.Should().BeNull();

            static PagedSearchDescriptor<TestDoc> Result;
            static ISearchResponse<TestDoc> Response;
            static PagedSearchDescriptor<TestDoc> PageDescriptor;
        }

        [Subject(typeof(SearchDescriptorPaging))]
        class When_next_page_for_not_last_response : SearchDescriptorPagingContext
        {
            Establish context = () =>
                {
                    PageDescriptor = new PagedSearchDescriptor<TestDoc>(x => x, 42, SearchDescriptorPaging.PageSize);
                    _expectedFrom = PageDescriptor.From + SearchDescriptorPaging.PageSize;

                    var mockResponse = new Mock<ISearchResponse<TestDoc>>();
                    mockResponse.SetupGet(r => r.Total).Returns(PageDescriptor.From + PageDescriptor.Size + 1);
                    Response = mockResponse.Object;
                };

            Because of = () => Result = Target.NextPage(PageDescriptor, Response);

            It should_set_from = () => Result.From.Should().Be(_expectedFrom);
            It should_set_size = () => Result.Size.Should().Be(SearchDescriptorPaging.PageSize);

            static PagedSearchDescriptor<TestDoc> Result;
            static ISearchResponse<TestDoc> Response;
            static PagedSearchDescriptor<TestDoc> PageDescriptor;
            static int _expectedFrom;
        }

        [Subject(typeof(SearchDescriptorPaging))]
        class When_create_page : SearchDescriptorPagingContext
        {
            Because of = () => Result = Target.CreatePage<TestDoc>(new NestQueryDsl().ByFieldValue("Id", 0));

            It should_init_search_descriptor = () => Result.SearchDescriptor.Should().NotBeNull();
            It should_set_from = () => Result.From.Should().Be(0);
            It should_set_size = () => Result.Size.Should().Be(SearchDescriptorPaging.PageSize);

            static PagedSearchDescriptor<TestDoc> Result;
        }

        class SearchDescriptorPagingContext
        {
            Establish context = () =>
            {
                Target = new SearchDescriptorPaging();
            };

            protected static SearchDescriptorPaging Target { get; private set; }
        }
    }
}