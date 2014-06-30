using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class OrDocsQueryBuilderSpecs
    {
        [Subject(typeof(OrDocsQueryBuilder))]
        class When_create_query : OrDocsQueryBuilderContext
        {
            Establish context = () =>
                {
                    _testEntity = new TestEntity();
                    var leftQuery = Mock.Of<IDocsQuery>();
                    var rightQuery = Mock.Of<IDocsQuery>();

                    LeftQueryBuilder.Setup(l => l.CreateQuery(_testEntity)).Returns(leftQuery);
                    RightQueryBuilder.Setup(r => r.CreateQuery(_testEntity)).Returns(rightQuery);

                    ExpectedQuery = Mock.Of<IDocsQuery>();

                    QueryDsl.Setup(q => q.Or(leftQuery, rightQuery)).Returns(ExpectedQuery);
                };

            Because of = () => Result = Target.CreateQuery(_testEntity);

            It should_create_or_query_for_left_and_right = () => Result.Should().Be(ExpectedQuery);

            protected static IDocsQuery ExpectedQuery;
            static IDocsQuery Result;
            static TestEntity _testEntity;
        }

        class OrDocsQueryBuilderContext
        {
            Establish context = () =>
                {
                    QueryDsl = new Mock<IQueryDsl>();
                    LeftQueryBuilder = new Mock<IDocsQueryBuilder>();
                    RightQueryBuilder = new Mock<IDocsQueryBuilder>();
                    Target = new OrDocsQueryBuilder(LeftQueryBuilder.Object, RightQueryBuilder.Object, QueryDsl.Object);
                };

            protected static Mock<IDocsQueryBuilder> RightQueryBuilder;
            protected static Mock<IDocsQueryBuilder> LeftQueryBuilder;
            protected static Mock<IQueryDsl> QueryDsl;
            protected static OrDocsQueryBuilder Target { get; private set; }
        }
    }
}