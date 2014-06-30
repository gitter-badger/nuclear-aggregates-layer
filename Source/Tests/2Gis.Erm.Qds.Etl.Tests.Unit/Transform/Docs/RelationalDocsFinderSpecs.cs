using System.Collections.Generic;

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
    // FIXME {f.zaharov, 24.04.2014}: нужно поднять читаймость и понятность тестов
    class RelationalDocsFinderSpecs
    {
        [Subject(typeof(RelationalDocsFinder))]
        class When_load_docs_by_linked_part : RelationalDocsFinderContext
        {
            Establish context = () =>
                {
                    _entity = Mock.Of<IEntityKey>();
                    _expectedDoc = new TestDoc();

                    var relation = new Mock<IDocRelation>();
                    var query = Mock.Of<IDocsQuery>();
                    relation.Setup(r => r.GetByPartQuery(_entity)).Returns(query);

                    DocsDataBase.Setup(db => db.Find<TestDoc>(query)).Returns(new[] { _expectedDoc });
                    QdsFactory.Setup(f => f.PartsDocRelation).Returns(relation.Object);
                };

            Because of = () => Result = Target.FindDocsByRelatedPart<TestDoc>(_entity);

            It should_load_docs_from_db_by_using_update_relation = () => Result.Should().OnlyContain(d => d == _expectedDoc);

            static IEnumerable<TestDoc> Result;
            static TestDoc _expectedDoc;
            static IEntityKey _entity;
        }

        class RelationalDocsFinderContext
        {
            Establish context = () =>
                {
                    QdsFactory = new Mock<IQdsComponent>();
                    DocsDataBase = new Mock<IDocsStorage>();

                    Target = new RelationalDocsFinder(QdsFactory.Object, DocsDataBase.Object);
                };

            protected static Mock<IDocsStorage> DocsDataBase { get; private set; }
            protected static Mock<IQdsComponent> QdsFactory { get; private set; }
            protected static RelationalDocsFinder Target { get; private set; }
        }
    }
}