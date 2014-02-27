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
                    FieldName = "FieldName";
                    ExpectedFieldName = "fieldName";
                    ExpectedValue = "value";
                };

            Because of = () => Result = Target.ByFieldValue(FieldName, ExpectedValue);

            It should_create_field_value_query = () => Result.Should().BeOfType<FieldValueQuery>();
            It should_init_field_name_as_camel_case = () => Result.As<FieldValueQuery>().FieldName.Should().Be(ExpectedFieldName);
            It should_init_field_value = () => Result.As<FieldValueQuery>().FieldValue.Should().Be(ExpectedValue);

            static IDocsQuery Result;
            static string ExpectedFieldName;
            static object ExpectedValue;
            static string FieldName;
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