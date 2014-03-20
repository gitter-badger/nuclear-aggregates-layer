using System;

using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticResponseHandlerSpecs
    {
        // “ест не требует запущенного Elastic'а, т.к. провер€ет функциональность обработки ошибок, пыта€сь подключитьс€ к несуществующему хосту.
        // Ќо при этом происходит попытка конекта, что довольно медленно, а потому пусть он будет интеграционным.
        [Subject(typeof(ElasticResponseHandler))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_refresh_with_invalid_host_name : ElasticClientContext
        {
            Establish contect = () =>
                {
                    CreateElasticClient("http://someinvalidhost:9200");
                    Response = ElasticClient.Refresh();
                };

            Because of = () => ResultException = HandleResponse();

            It should_throw_appropriate_exception = () => ResultException.Should().NotBeNull().And.BeOfType<ElasticException>();
            It should_init_message_from_response_error = () => ResultException.Message.Should().Be(Response.ConnectionStatus.Error.ExceptionMessage);
            It should_init_inner_by_response_original_exception = () => ResultException.InnerException.Should().Be(Response.ConnectionStatus.Error.OriginalException);
        }

        [Subject(typeof(ElasticResponseHandler))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_create_index_with_upper_case : ElasticClientContext
        {
            Establish contect = () =>
            {
                CreateElasticClient("http://localhost:9200");
                Response = ElasticClient.CreateIndex("UpperNameIndex", new IndexSettings());
            };

            Because of = () => ResultException = HandleResponse();

            It should_init_message_from_result_because_it_more_informative_message = () => ResultException.Message.Should().Be(Response.ConnectionStatus.Result);
        }

        [Subject(typeof(ElasticResponseHandler))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_refresh_with_valid_host_name : ElasticClientContext
        {
            Establish contect = () =>
            {
                CreateElasticClient("http://localhost:9200");
                Response = ElasticClient.Refresh();
            };

            Because of = () => ResultException = HandleResponse();

            It should_not_throw_exceptions = () => ResultException.Should().BeNull();
        }

        class ElasticClientContext
        {
            Establish contect = () => Target = new ElasticResponseHandler();
            Cleanup cleanup = () => { if (ElasticClient != null) ElasticClient.DeleteIndex(ElasticTestConfigurator.TestIndexName); };

            protected static Exception HandleResponse()
            {
                return Catch.Exception(() => Target.ThrowWhenError(Response));
            }

            protected static void CreateElasticClient(string host)
            {
                ElasticClient = ElasticTestConfigurator.CreateElasticClient(host);
            }

            protected static ElasticResponseHandler Target { get; private set; }
            protected static IElasticClient ElasticClient { get; private set; }

            protected static IResponse Response;
            protected static Exception ResultException;
        }
    }
}