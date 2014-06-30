using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit;
using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Integration
{
    class ElasticDocsStorageSpecs
    {
        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_find_in_nested_multiplicity : NestedSeacrhElasticDocsStorageContext
        {
            Establish context = () => Target.Update(
                new[] { 
                    new TestDoc("1", "", new NestedDoc{Id="22"}), 
                    new TestDoc("2", "", new NestedDoc{Id="2"}), 
                    new TestDoc("3", "", new NestedDoc{Id="2"}), 
                    new TestDoc("4", "", new NestedDoc{Id="3"}) 
                });

            Because of = () => Result = Target.Find<TestDoc>(new NestQueryDsl().ByNestedObjectQuery("NestedDocs", new NestQueryDsl().FieldInQuery("Id", "2", "3")));

            It shoul_find_only_two_documents = () => Result.Should().HaveCount(3);
            It shoul_find_only_expected_docs = () => Result.Should().OnlyContain(d => d.Id == "2" || d.Id == "3" || d.Id == "4");
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_find_by_nested_or_nested_nested_query : NestedSeacrhElasticDocsStorageContext
        {
            Establish context = () =>
            {
                MoreNestedId = "4242";
                NestedId = "5252";

                ExpectedId = "42";
                AnotherExpectedId = "52";

                AddTestDoc(ExpectedId, CreateNestedDoc("888", new MoreNestedDoc { Id = MoreNestedId }));
                AddTestDoc(AnotherExpectedId, CreateNestedDoc(NestedId));

                AddTestDoc("111", CreateNestedDoc("555", new MoreNestedDoc { Id = "333" }));
            };


            Because of = () => Result = Target.Find<TestDoc>(CreateOrObjectQuery(
                CreateByNestedObjectQuery(NestedPropertyName, CreateByFieldQuery("Id", NestedId)),
                CreateByNestedObjectQuery(NestedPropertyName, CreateByNestedObjectQuery("MoreNestedDocs", CreateByFieldQuery("Id", MoreNestedId)))));

            It shoul_find_only_two_documents = () => Result.Should().HaveCount(2);
            It should_return_doc_with_expected_id = () => Result.Should().Contain(d => d.Id == ExpectedId);
            It should_return_doc_with_another_expected_id = () => Result.Should().Contain(d => d.Id == AnotherExpectedId);

            static string NestedId;
            static string MoreNestedId;
            static string AnotherExpectedId;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_find_by_nested_nested_query : NestedSeacrhElasticDocsStorageContext
        {
            Establish context = () =>
            {
                MoreNestedId = "1231";
                ExpectedId = "42";

                AddTestDoc(ExpectedId, CreateNestedDoc("887", new MoreNestedDoc { Id = MoreNestedId }));
                AddTestDoc("122", CreateNestedDoc("987", new MoreNestedDoc { Id = "33" }));
            };

            Because of = () => Result = Target.Find<TestDoc>(CreateByNestedObjectQuery(NestedPropertyName, CreateByNestedObjectQuery("MoreNestedDocs", CreateByFieldQuery("Id", MoreNestedId))));

            It should_return_doc_with_id = () => Result.Should().OnlyContain(d => d.Id == ExpectedId);

            static string MoreNestedId;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_find_by_nested_query : NestedSeacrhElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    NestedId = "333";
                    ExpectedId = "42";

                    AddTestDoc(ExpectedId, CreateNestedDoc(NestedId));
                    AddTestDoc("777", CreateNestedDoc("123"));
                };

            Because of = () => Result = Target.Find<TestDoc>(CreateByNestedObjectQuery(NestedPropertyName, CreateByFieldQuery("Id", NestedId)));

            It should_return_doc_with_id = () => Result.Should().OnlyContain(d => d.Id == ExpectedId);

            protected static string NestedId;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_update_docs_more_than_find_page_size : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    SearchValue = "searchvalue";
                    ExpectedCount = SearchDescriptorPaging.PageSize * 2 + SearchDescriptorPaging.PageSize / 2;

                    var testDocs = new TestDoc[ExpectedCount];

                    for (int i = 0; i < ExpectedCount; i++)
                        testDocs[i] = new TestDoc(i.ToString(), SearchValue);

                    Target.Update(testDocs);
                };

            Because of = () => Result = Target.Find<TestDoc>(CreateByFieldQuery("TextValue", SearchValue));

            It should_find_all_docs = () => Result.Should().HaveCount(ExpectedCount);
            It should_only_expected_docs = () => Result.Should().OnlyContain(d => d.TextValue == SearchValue);

            static IEnumerable<TestDoc> Result;
            private static int ExpectedCount;
            static string SearchValue;
        }

        [Subject(typeof(ElasticDocsStorage))]
        [Tags(TestTags.IntegrationTestTag, TestTags.ElasticTestTag)]
        class When_update_doc : ElasticDocsStorageContext
        {
            Establish context = () =>
                {
                    ExpectedId = "42";
                    ExpectedValue = "some value";
                };

            Because of = () => Target.Update(new[] { new TestDoc(ExpectedId, ExpectedValue) });

            It should_be_findable_by_field_value = () => Target.Find<TestDoc>(CreateByFieldQuery("Id", ExpectedId))
                .Should().OnlyContain(d => d.Id == ExpectedId.ToString() && d.TextValue == ExpectedValue);

            static string ExpectedId;
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

        class NestedSeacrhElasticDocsStorageContext : ElasticDocsStorageContext
        {
            Establish context = () => MapTestDocWithNested();

            protected static void AddTestDoc(string id, params NestedDoc[] nested)
            {
                Target.Update(new[] { new TestDoc { Id = id, NestedDocs = nested } });
            }

            protected static IDocsQuery CreateOrObjectQuery(IDocsQuery left, IDocsQuery right)
            {
                return new NestQueryDsl().Or(left, right);
            }

            protected static IDocsQuery CreateByNestedObjectQuery(string nestedName, IDocsQuery nestedQuery)
            {
                return new NestQueryDsl().ByNestedObjectQuery(nestedName, nestedQuery);
            }

            protected static NestedDoc CreateNestedDoc(string id, params MoreNestedDoc[] nestedDocs)
            {
                return new NestedDoc { Id = id, MoreNestedDocs = nestedDocs };
            }

            static void MapTestDocWithNested()
            {
                ElasticManagementApi.Map<TestDoc>(m => m
                    .Properties(p => p
                        .NestedObject<NestedDoc>(nested => nested
                            .Name(n => n.NestedDocs.First())
                            .Properties(pp => pp
                                .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                .NestedObject<MoreNestedDoc>(nestedNested => nestedNested
                                    .Name(n => n.MoreNestedDocs.First())
                                    .Properties(ppp => ppp
                                        .String(s => s.Name(n => n.Id).Index(FieldIndexOption.not_analyzed))
                                    )
                                )
                            )
                        )
                    )
                );

                ElasticApi.Refresh();
            }

            protected static string ExpectedId;
            protected static IEnumerable<TestDoc> Result;
            protected const string NestedPropertyName = "NestedDocs";
        }

        class ElasticDocsStorageContext : ElasticApiIntegrationContext
        {
            Establish context = () =>
            {
                ElasticTestConfigurator.RegisterDocumentAndCreateIndex<TestDoc>(NestSettings, ElasticManagementApi);
                Target = new ElasticDocsStorage(ElasticApi, new SearchDescriptorPaging());
            };

            Cleanup cleanup = () => ElasticManagementApi.DeleteIndex<TestDoc>();

            protected static IDocsQuery CreateByFieldQuery(string fieldName, object value)
            {
                return new NestQueryDsl().ByFieldValue(fieldName, value);
            }

            protected static ElasticDocsStorage Target { get; private set; }
        }
    }
}
