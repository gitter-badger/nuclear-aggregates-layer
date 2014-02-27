using System;

using FluentAssertions;

using Machine.Specifications;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticClientSpecs
    {
        // “ест не требует запущенного Elastic'а, т.к. провер€ет функциональность обработки ошибок, пыта€сь подключитьс€ к несуществующему хосту.
        // Ќо при этом происходит попытка конекта, что довольно медленно, а потому пусть он будет интеграционным.
        [Subject(typeof(ElasticClient))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_refresh_with_invalid_host_name : ElasticClientContext
        {
            Establish contect = () => CreateElasticClient("http://someinvalidhost:9200");

            Because of = () => Result = Target.Refresh();

            It result_is_valid_should_be_false = () => Result.IsValid.Should().BeFalse();
            It should_contain_exception_message = () => Result.ConnectionStatus.Error.ExceptionMessage.Should().NotBeNull();
            It should_contain_original_exception = () => Result.ConnectionStatus.Error.OriginalException.Should().NotBeNull();

            static IResponse Result;
        }

        [Subject(typeof(ElasticClient))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_refresh_with_valid_host_name : ElasticClientContext
        {
            Establish contect = () => CreateElasticClient("http://localhost:9200");

            Because of = () => Result = Target.Refresh();

            It should_be_true = () => Result.OK.Should().BeTrue();

            static IIndicesShardResponse Result;
        }

        class ElasticClientContext
        {
            protected static IElasticClient Target { get; private set; }

            protected static void CreateElasticClient(string host)
            {
                Target = ElasticTestConfigurator.CreateElasticClient(host);
            }

            Cleanup cleanup = () => { if (Target != null) Target.DeleteIndex(ElasticTestConfigurator.TestIndexName); };
        }
    }
}