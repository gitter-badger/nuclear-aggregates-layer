using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class FieldsEqualsDocsQueryBuilderSpecs
    {
        [Subject(typeof(FieldsEqualsDocsQueryBuilder<,>))]
        class When_create_query : FieldsEqualsDocsQueryBuilderContext<TestDoc, TestEntity>
        {
            Establish context = () =>
                {
                    _expectedQuery = Mock.Of<IDocsQuery>();
                    const string expectedValue = "some value";

                    _testEntity = new TestEntity { PropertyOne = expectedValue };

                    CreateTarget(d => d.PropertyOneDoc, p => p.PropertyOne);
                    QueryDsl.Setup(dsl => dsl.ByFieldValue("PropertyOneDoc", expectedValue)).Returns(_expectedQuery);
                };

            Because of = () => Result = Target.CreateQuery(_testEntity);

            It should_return_by_field_value_query = () => Result.Should().Be(_expectedQuery);

            static IEntityKey _testEntity;
            static IDocsQuery _expectedQuery;
            static IDocsQuery Result;
        }

        class FieldsEqualsDocsQueryBuilderContext<TDoc, TPart> where TPart : IEntityKey
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