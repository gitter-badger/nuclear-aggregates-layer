using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class RelationalDocsUpdaterSpecs
    {
        [Subject(typeof(RelationalDocsUpdater<>))]
        class When_create_new_doc_returns_null : RelationalDocsUpdaterContext
        {
            Establish context = () =>
            {
                TestEntity = new TestEntity();

                DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(TestEntity)).Returns(new TestDoc[0]);
                QdsComponent.Setup(f => f.CreateNewDoc(TestEntity)).Returns((IDoc)null);

                Entities = new IEntityKey[] { TestEntity };
            };

            Because of = () => Target.UpdateDocuments(Entities).ToArray();

            It should_call_update_with_empty_docs_list = () =>
                DocMapper.Verify(m => m.UpdateDocByEntity(Moq.It.Is<IEnumerable<TestDoc>>(docs => !docs.Any()), TestEntity));

            static TestEntity TestEntity;
            static IEnumerable<IEntityKey> Entities { get; set; }
        }


        [Subject(typeof(RelationalDocsUpdater<>))]
        class When_update_docs_by_new_entity : RelationalDocsUpdaterContext
        {
            Establish context = () =>
            {
                TestEntity = new TestEntity();
                ExpectedDoc = new TestDoc();

                DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(TestEntity)).Returns(new TestDoc[0]);
                QdsComponent.Setup(f => f.CreateNewDoc(TestEntity)).Returns(ExpectedDoc);

                Entities = new IEntityKey[] { TestEntity };
            };

            Because of = () => Target.UpdateDocuments(Entities).ToArray();

            It should_update_new_doc = () =>
                DocMapper.Verify(m => m.UpdateDocByEntity(Moq.It.Is<IEnumerable<TestDoc>>(docs => docs.Contains(ExpectedDoc)), TestEntity));

            static TestDoc ExpectedDoc;
            static TestEntity TestEntity;

            static IEnumerable<IEntityKey> Entities { get; set; }
        }

        [Subject(typeof(RelationalDocsUpdater<>))]
        class When_update_docs_by_entities : RelationalDocsUpdaterContext
        {
            Establish context = () =>
                {
                    TestEntity = new TestEntity();
                    AnotherTestEntity = new AnotherTestEntity();

                    ExpectedDocs = new[] { new TestDoc(), };
                    ExpectedAnotherDocs = new[] { new TestDoc(), };

                    DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(TestEntity)).Returns(ExpectedDocs);
                    DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(AnotherTestEntity)).Returns(ExpectedAnotherDocs);

                    Entities = new IEntityKey[] { TestEntity, AnotherTestEntity };
                };

            Because of = () => Target.UpdateDocuments(Entities).ToArray();

            It should_map_each_entity_to_docs = () =>
                {
                    DocMapper.Verify(m => m.UpdateDocByEntity(ExpectedDocs, TestEntity));
                    DocMapper.Verify(m => m.UpdateDocByEntity(ExpectedAnotherDocs, AnotherTestEntity));
                };

            static IEnumerable<TestDoc> ExpectedDocs;
            private static IEnumerable<TestDoc> ExpectedAnotherDocs;
            private static TestEntity TestEntity;
            private static AnotherTestEntity AnotherTestEntity;

            static IEnumerable<IEntityKey> Entities { get; set; }
        }

        [Subject(typeof(RelationalDocsUpdater<>))]
        class When_constructed : RelationalDocsUpdaterContext
        {
            It should_init_supported_doc_type_by_generic_parameter_type = () => Target.SupportedDocType.Should().Be(typeof(TestDoc));
        }

        class RelationalDocsUpdaterContext
        {
            Establish context = () =>
                {
                    DocCatalog = new Mock<IRelationalDocsFinder>();
                    DocMapper = new Mock<IDocMapper<TestDoc>>();
                    QdsComponent = new Mock<IQdsComponent>();

                    Target = new RelationalDocsUpdater<TestDoc>(QdsComponent.Object, DocCatalog.Object, DocMapper.Object);
                };

            protected static Mock<IDocMapper<TestDoc>> DocMapper { get; private set; }
            protected static Mock<IQdsComponent> QdsComponent { get; private set; }
            protected static Mock<IRelationalDocsFinder> DocCatalog { get; private set; }

            protected static RelationalDocsUpdater<TestDoc> Target { get; private set; }
        }
    }
}