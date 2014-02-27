using System;

using DoubleGis.Erm.Qds;

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
        class When_response_valid : ElasticResponseHandlerContext
        {
            Establish context = () =>
                {
                    TestResponse = new Mock<IResponse>();
                    TestResponse.SetupGet(r => r.IsValid).Returns(true);
                };

            Because of = () => Result = Catch.Exception(() => Target.ThrowWhenError(TestResponse.Object));

            It should_not_throw = () => Result.Should().BeNull();

            static Mock<IResponse> TestResponse;
            static Exception Result;
        }

        [Subject(typeof(ElasticResponseHandler))]
        class When_error_response : ElasticResponseHandlerContext
        {
            Establish context = () =>
                {
                    _expectedMessage = "some error message";
                    _expectedException = new Exception(_expectedMessage);

                    var connStatus = new ConnectionStatus(Mock.Of<IConnectionSettings>(), _expectedException);

                    TestResponse = new Mock<IResponse>();
                    TestResponse.SetupGet(r => r.ConnectionStatus).Returns(connStatus);
                };

            Because of = () => Result = Catch.Exception(() => Target.ThrowWhenError(TestResponse.Object));

            It should_throw_docs_storage_exception = () => Result.Should().NotBeNull().And.BeOfType<DocsStorageException>();
            It should_contain_original_exception = () => Result.InnerException.Should().Be(_expectedException);
            It should_contain_message_from_response = () => Result.Message.Should().Be(_expectedMessage);

            static Mock<IResponse> TestResponse;
            static Exception Result;
            static string _expectedMessage;
            private static Exception _expectedException;
        }

        class ElasticResponseHandlerContext
        {
            Establish context = () =>
                {
                    Target = new ElasticResponseHandler();
                };

            protected static ElasticResponseHandler Target { get; private set; }
        }
    }
}