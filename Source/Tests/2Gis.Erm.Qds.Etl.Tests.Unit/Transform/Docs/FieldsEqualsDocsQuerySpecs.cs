using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class FieldsEqualsDocsQuerySpecs
    {
        [Subject(typeof(FieldsEqualsDocsQueryBuilder<,>))]
        class When_create_query : FieldsEqualsDocsQueryContext<TestDoc, TestEntity>
        {
            Establish context = () =>
                {
                    _expectedQuery = Mock.Of<IDocsQuery>();
                    _expectedValue = "some value";

                    _testEntity = new TestEntity { PropertyOne = _expectedValue };

                    CreateTarget(d => d.PropertyOneDoc, p => p.PropertyOne);
                    QueryDsl.Setup(dsl => dsl.ByFieldValue("PropertyOneDoc", _expectedValue)).Returns(_expectedQuery);
                };

            Because of = () => Result = Target.CreateQuery(_testEntity);

            It should_return_by_field_value_query = () => Result.Should().Be(_expectedQuery);

            static IEntityKey _testEntity;
            static IDocsQuery _expectedQuery;
            static string _expectedValue;
            static IDocsQuery Result;
        }

        //[Subject(typeof(FieldsEqualsDocsQueryBuilder<,>))]
        //class When_construct_with_doc_and_part_fields_expression : FieldsEqualsDocsQueryContext<TestDoc, TestEntity>
        //{
        //    Because of = () => CreateTarget(d => d.PropertyOneDoc, p => p.PropertyOne);

        //    It should_init_doc_field_name = () => Target.DocFieldName.Should().Be("PropertyOneDoc");
        //    It should_init_part_field_name = () => Target.PartFieldName.Should().Be("PropertyOne");
        //}

        class FieldsEqualsDocsQueryContext<TDoc, TPart> where TPart : IEntityKey
        {
            Establish context = () =>
                {
                    QueryDsl = new Mock<IQueryDsl>();
                };

            protected static void CreateTarget(Expression<Func<TDoc, object>> docField, Expression<Func<TPart, object>> partField)
            {
                Target = new FieldsEqualsDocsQueryBuilder<TDoc, TPart>(docField, partField, QueryDsl.Object);
            }

            protected static Mock<IQueryDsl> QueryDsl { get; private set; }

            protected static FieldsEqualsDocsQueryBuilder<TDoc, TPart> Target { get; private set; }
        }
    }
}