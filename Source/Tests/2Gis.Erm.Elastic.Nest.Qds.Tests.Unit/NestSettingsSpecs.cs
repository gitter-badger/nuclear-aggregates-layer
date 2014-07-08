using FluentAssertions;

using Machine.Specifications;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class NestSettingsSpecs
    {
        [Subject(typeof(NestSettingsAspect))]
        class When_register_document_type
        {
            const string IndexPrefix = "IndexPrefix";
            const string IndexName = "IndexName";
            const string DocName = "TestDoc1";

            const string ExpectedIndexName = "indexprefix.indexname";
            const string ExpectedDocName = "testdoc1";

            Establish context = () =>
            {
                const string ConnectionString = "Uris=['http://localhost:9200'];IndexPrefix=" + IndexPrefix;
                Target = new NestSettingsAspect(ConnectionString);
                Result = Target.ConnectionSettings;
            };

            Because of = () => Target.RegisterType<TestDoc>(IndexName, DocName);

            It contains_index_for_document_type = () => Result.DefaultIndices.Should().ContainKey(typeof(TestDoc));
            It correct_index_name = () => Result.DefaultIndices[typeof(TestDoc)].ShouldBeEquivalentTo(ExpectedIndexName);
            It contains_document_for_document_type = () => Result.DefaultTypeNames.Should().ContainKey(typeof(TestDoc));
            It correct_document_name = () => Result.DefaultTypeNames[typeof(TestDoc)].ShouldBeEquivalentTo(ExpectedDocName);

            private static IConnectionSettingsValues Result { get; set; }
            private static NestSettingsAspect Target { get; set; }

            private sealed class TestDoc { }
        }
    }
}