using System.Collections.Generic;

using DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit;
using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticDocsStorageSpecs
    {
        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_update_docs_more_than_find_page_size : ElasticDocsStorageContext
        {
            Establish context = () =>
            {
                ExpectedCount = ElasticMeta.PageSize * 2 + 1;
                var testDocs = new TestDoc[ExpectedCount];

                for (int i = 0; i < ExpectedCount; i++)
                    testDocs[i] = new TestDoc(i, "searchvalue");

                Target.Update(testDocs);
            };

            Because of = () => Result = Target.Find<TestDoc>(CreateByFieldQuery("TextValue", "searchvalue"));

            It should_find_all_docs = () => Result.Should().HaveCount(ExpectedCount);
            It should_only_expected_docs = () => Result.Should().OnlyContain(d => d.TextValue == "searchvalue");

            static IEnumerable<TestDoc> Result;
            private static int ExpectedCount;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_update_doc : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ExpectedId = 42;
                    ExpectedValue = "some value";
                };

            Because of = () => Target.Update(new[] { new TestDoc(ExpectedId, ExpectedValue) });

            It should_be_findable_by_field_value = () => Target.Find<TestDoc>(CreateByFieldQuery("Id", ExpectedId))
                .Should().OnlyContain(d => d.Id == ExpectedId && d.TextValue == ExpectedValue);

            static int ExpectedId;
            static string ExpectedValue;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_find_not_existing_docs : ElasticDocsStorageContext
        {
            Because of = () => Result = Target.Find<TestDoc>(CreateByFieldQuery("Id", 0));

            It should_return_empty = () => Result.Should().BeEmpty();

            static IEnumerable<IDoc> Result;
        }

        class ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ElasticClient = ElasticTestConfigurator.CreateElasticClient("http://localhost:9200");

                    ElasticClient.DeleteIndex(ElasticTestConfigurator.TestIndexName);
                    ElasticClient.CreateIndex(ElasticTestConfigurator.TestIndexName);

                    var settingsFactory = new Mock<IElasticConnectionSettingsFactory>();
                    settingsFactory.Setup(sf => sf.GetIsolatedIndexName(Moq.It.IsAny<string>())).Returns(ElasticTestConfigurator.TestIndexName);

                    Target = new ElasticDocsStorage(new MockElasticClientFactory(ElasticClient), new ElasticMeta(settingsFactory.Object), new ElasticResponseHandler());
                };

            Cleanup cleanup = () => ElasticClient.DeleteIndex(ElasticTestConfigurator.TestIndexName);

            protected static IDocsQuery CreateByFieldQuery(string fieldName, object value)
            {
                return new NestQueryDsl().ByFieldValue(fieldName, value);
            }

            protected static IElasticClient ElasticClient { get; private set; }
            protected static ElasticDocsStorage Target { get; private set; }
        }
    }
}
