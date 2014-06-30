using System.Linq;

using DoubleGis.Erm.Qds;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class NestQueryDslSpecs
    {
        [Subject(typeof(NestQueryDsl))]
        class When_by_field_in_query : NestQueryDslContext
        {
            Establish context = () =>
            {
                FieldName = "FieldName";
                ExpectedFieldName = "fieldName";
                ExpectedValue = new[] { "1", "2" };
            };

            Because of = () => Result = Target.FieldInQuery(FieldName, ExpectedValue);

            It should_create_field_value_query = () => Result.Should().BeOfType<FieldInQuery>();
            It should_init_field_name_as_camel_case = () => Result.As<FieldInQuery>().FieldName.Should().Be(ExpectedFieldName);
            It should_init_field_value = () => Result.As<FieldInQuery>().Terms.Should().Equal(ExpectedValue.Cast<object>());

            static string ExpectedFieldName;
            static string[] ExpectedValue;
            static string FieldName;
        }

        [Subject(typeof(NestQueryDsl))]
        class When_by_or_object_query : NestQueryDslContext
        {
            Establish context = () =>
            {
                ExpectedLeftQuery = Mock.Of<IDocsQuery>();
                ExpectedRightQuery = Mock.Of<IDocsQuery>();
            };

            Because of = () => Result = Target.Or(ExpectedLeftQuery, ExpectedRightQuery);

            It should_create_nested_object_query = () => Result.Should().BeOfType<OrObjectQuery>();
            It should_init_nested_object_name_as_camel_case = () => Result.As<OrObjectQuery>().Left.Should().Be(ExpectedLeftQuery);
            It should_init_nested_query = () => Result.As<OrObjectQuery>().Right.Should().Be(ExpectedRightQuery);

            static IDocsQuery ExpectedRightQuery;
            static IDocsQuery ExpectedLeftQuery;
        }

        [Subject(typeof(NestQueryDsl))]
        class When_by_nested_object_query : NestQueryDslContext
        {
            Establish context = () =>
                {
                    ExpectedNestedObjectName = "nestedObject";
                    ExpectedNestedQuery = Mock.Of<IDocsQuery>();
                };

            Because of = () => Result = Target.ByNestedObjectQuery(ExpectedNestedObjectName, ExpectedNestedQuery);

            It should_create_nested_object_query = () => Result.Should().BeOfType<NestedObjectQuery>();
            It should_init_nested_object_name_as_camel_case = () => Result.As<NestedObjectQuery>().NestedObjectName.Should().Be(ExpectedNestedObjectName);
            It should_init_nested_query = () => Result.As<NestedObjectQuery>().NestedQuery.Should().Be(ExpectedNestedQuery);

            static IDocsQuery ExpectedNestedQuery;
            static string ExpectedNestedObjectName;
        }

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

            protected static IDocsQuery Result;
        }
    }
}