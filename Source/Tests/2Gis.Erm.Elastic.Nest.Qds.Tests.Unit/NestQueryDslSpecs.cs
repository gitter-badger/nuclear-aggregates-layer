using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class NestQueryDslSpecs
    {
        [Subject(typeof(NestQueryDsl))]
        class When_by_field_name_query : NestQueryDslContext
        {
            Establish context = () =>
                {
                    ExpectedFieldName = "fieldname";
                    ExpectedValue = "value";
                };

            Because of = () => Result = Target.ByFieldValue(ExpectedFieldName, ExpectedValue);

            It should_create_field_value_query = () => Result.Should().BeOfType<FieldValueQuery>();
            It should_init_field_name = () => Result.As<FieldValueQuery>().FieldName.Should().Be(ExpectedFieldName);
            It should_init_field_value = () => Result.As<FieldValueQuery>().FieldValue.Should().Be(ExpectedValue);

            static IDocsQuery Result;
            static string ExpectedFieldName;
            static object ExpectedValue;
        }

        class NestQueryDslContext
        {
            Establish context = () =>
                {
                    Target = new NestQueryDsl();
                };

            protected static NestQueryDsl Target { get; private set; }
        }
    }
}