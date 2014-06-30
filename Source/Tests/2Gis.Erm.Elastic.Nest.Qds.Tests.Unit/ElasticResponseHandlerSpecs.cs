using System;

using FluentAssertions;
using Machine.Specifications;
using Moq;
using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class ElasticResponseHandlerSpecs
    {
        [Subject(typeof(ElasticResponseHandler))]
        class When_bulk_response_is_valid_but_inner_response_is_invalid : ElasticResponseHandlerContext<IBulkResponse>
        {
            const string ErrorText = "error";

            Establish context = () =>
            {
                var responseItem = new BulkIndexResponseItem();
                // internal property, so only reflection can be used for mocking
                typeof(BulkIndexResponseItem).GetProperty("Error").SetValue(responseItem, ErrorText);

                Response.SetupGet(x => x.IsValid).Returns(true);
                Response.SetupGet(x => x.Errors).Returns(true);
                Response.SetupGet(x => x.Items).Returns(new[] { responseItem, responseItem });
            };

            Because of = () => Exception = Catch.Exception(() => Target.ThrowWhenError(Response.Object));

            It should_throw_error = () => Exception.Should().BeOfType<ElasticException>();
            It should_contains_error_text = () => Exception.Message.Should().Contain(ErrorText);
        }

        [Subject(typeof(ElasticResponseHandler))]
        class When_bulk_response_is_valid_and_inner_responses_valid : ElasticResponseHandlerContext<IBulkResponse>
        {
            Establish context = () =>
            {
                Response.SetupGet(x => x.IsValid).Returns(true);
                Response.SetupGet(x => x.Errors).Returns(false);
            };

            Because of = () => Exception = Catch.Exception(() => Target.ThrowWhenError(Response.Object));

            It should_not_throw_error = () => Exception.Should().BeNull();
        }

        [Subject(typeof(ElasticResponseHandler))]
        class When_response_is_valid_true : ElasticResponseHandlerContext<IResponse>
        {
            Establish context = () => Response.SetupGet(x => x.IsValid).Returns(true);

            Because of = () => Exception = Catch.Exception(() => Target.ThrowWhenError(Response.Object));

            It should_throw_exception = () => Exception.Should().BeNull();
        }

        [Subject(typeof(ElasticResponseHandler))]
        class When_bulk_response_is_valid_false : IsValidFalseElasticResponseHandlerContext<IBulkResponse>
        {
            Because of = () => Exception = Catch.Exception(() => Target.ThrowWhenError(Response.Object));

            It should_throw_exception = () => ValidateExceptionState();
        }

        [Subject(typeof(ElasticResponseHandler))]
        class When_response_is_valid_false : IsValidFalseElasticResponseHandlerContext<IResponse>
        {
            Because of = () => Exception = Catch.Exception(() => Target.ThrowWhenError(Response.Object));

            It should_throw_exception = () => ValidateExceptionState();
        }

        class IsValidFalseElasticResponseHandlerContext<TResponse> : ElasticResponseHandlerContext<TResponse> where TResponse : class, IResponse
        {
            Establish context = () =>
            {
                ExpectedInner = new Exception();
                ExpectedMessage = "Some error message";
                var connStatus = new StubElasticsearchResponse
                {
                    OriginalException = ExpectedInner,
                    ToStringResult = ExpectedMessage,
                };

                Response.SetupGet(x => x.IsValid).Returns(false);
                Response.SetupGet(r => r.ConnectionStatus).Returns(connStatus);
            };

            protected static void ValidateExceptionState()
            {
                Exception.Should().BeOfType<ElasticException>();
                Exception.InnerException.Should().Be(ExpectedInner);
                Exception.Message.Should().Be(ExpectedMessage);
            }

            protected static Exception ExpectedInner;
            protected static string ExpectedMessage;
        }

        class ElasticResponseHandlerContext<TResponse> where TResponse : class, IResponse
        {
            Establish context = () =>
                {
                    Target = new ElasticResponseHandler();
                    Response = new Mock<TResponse>();
                };

            protected static ElasticResponseHandler Target { get; private set; }
            protected static Exception Exception;
            protected static Mock<TResponse> Response;
        }
    }
}
