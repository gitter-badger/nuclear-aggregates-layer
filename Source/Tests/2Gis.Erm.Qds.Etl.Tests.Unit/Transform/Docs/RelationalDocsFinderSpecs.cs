using System;
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
    class RelationalDocsFinderSpecs
    {
        [Subject(typeof(RelationalDocsFinder))]
        class When_load_docs_associative_with_no_relation : RelationalDocsFinderContext
        {
            Establish context = () =>
            {
                _part = Mock.Of<IEntityKey>();

                var relation = new Mock<IDocRelation>();
                relation.Setup(r => r.GetDocType()).Returns(Mock.Of<Type>);
            };

            Because of = () => Result = Catch.Exception(() => Target.FindDocsByIndirectRelationPart<TestDoc>(_part));

            It should_throw_not_supported_exception = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
            static IEntityKey _part;
        }

        [Subject(typeof(RelationalDocsFinder))]
        class When_load_docs_associative : RelationalDocsFinderContext
        {
            Establish context = () =>
            {
                _part = Mock.Of<IEntityKey>();
                _expectedDoc = new TestDoc();

                var relation = new Mock<IDocRelation>();
                relation.Setup(r => r.GetDocType()).Returns(typeof(TestDoc));

                var query = Mock.Of<IDocsQuery>();
                relation.Setup(r => r.GetByPartQuery(_part)).Returns(query);

                DocsDataBase.Setup(db => db.Find<TestDoc>(query)).Returns(new[] { _expectedDoc });
                QdsFactory.Setup(f => f.IndirectDocRelations).Returns(new[] { relation.Object });
            };

            Because of = () => Result = Target.FindDocsByIndirectRelationPart<TestDoc>(_part);

            It should_load_docs_from_db_by_using_build_relation = () => Result.Should().OnlyContain(d => d == _expectedDoc);

            static IEnumerable<TestDoc> Result;
            static TestDoc _expectedDoc;
            static IEntityKey _part;
        }

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