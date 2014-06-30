using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class NestedDocsQueryBuilderSpecs
    {
        [Subject(typeof(NestedDocsQueryBuilder<>))]
        class When_create_query : NestedDocsQueryBuilderContext
        {
            Establish context = () =>
                {
                    _testEntity = new TestEntity();
                    var nestedQuery = Mock.Of<IDocsQuery>();
                    ExpectedQuery = Mock.Of<IDocsQuery>();

                    NestedQueryBuilder.Setup(nq => nq.CreateQuery(_testEntity)).Returns(nestedQuery);

                    QueryDsl.Setup(q => q.ByNestedObjectQuery("PropertyOneDoc", nestedQuery)).Returns(ExpectedQuery);
                };

            Because of = () => Result = Target.CreateQuery(_testEntity);

            It should_nested = () => Result.Should().Be(ExpectedQuery);

            static TestEntity _testEntity;
            static IDocsQuery Result;
            static IDocsQuery ExpectedQuery;
        }

        class NestedDocsQueryBuilderContext
        {
            Establish context = () =>
                {
                    NestedQueryBuilder = new Mock<IDocsQueryBuilder>();
                    QueryDsl = new Mock<IQueryDsl>();

                    Target = new NestedDocsQueryBuilder<TestDoc>(d => d.PropertyOneDoc, NestedQueryBuilder.Object, QueryDsl.Object);
                };

            protected static Mock<IDocsQueryBuilder> NestedQueryBuilder;
            protected static Mock<IQueryDsl> QueryDsl;

            protected static NestedDocsQueryBuilder<TestDoc> Target { get; private set; }
        }
    }
}