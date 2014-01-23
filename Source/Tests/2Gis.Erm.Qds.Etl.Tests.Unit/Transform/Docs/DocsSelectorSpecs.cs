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
    class DocsSelectorSpecs
    {
        [Subject(typeof(DocsSelector<>))]
        class When_create_new_doc_returns_null : DocsSelectorContext
        {
            Establish context = () =>
            {
                _testEntity = new TestEntity();

                DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(_testEntity)).Returns(new TestDoc[0]);
                QdsComponent.Setup(f => f.CreateNewDoc(_testEntity)).Returns((IDoc)null);

                Entities = new IEntityKey[] { _testEntity };
            };

            Because of = () => Target.ModifyDocuments(Entities).ToArray();

            It should_call_update_with_empty_docs_list = () =>
                DocMapper.Verify(m => m.UpdateDocByEntity(Moq.It.Is<IEnumerable<TestDoc>>(docs => !docs.Any()), _testEntity));

            static TestEntity _testEntity;
            static IEnumerable<IEntityKey> Entities { get; set; }
        }


        [Subject(typeof(DocsSelector<>))]
        class When_modify_docs_by_new_entity : DocsSelectorContext
        {
            Establish context = () =>
            {
                _testEntity = new TestEntity();
                _expectedDoc = new TestDoc();

                DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(_testEntity)).Returns(new TestDoc[0]);
                QdsComponent.Setup(f => f.CreateNewDoc(_testEntity)).Returns(_expectedDoc);

                Entities = new IEntityKey[] { _testEntity };
            };

            Because of = () => Target.ModifyDocuments(Entities).ToArray();

            It should_update_new_doc = () =>
                DocMapper.Verify(m => m.UpdateDocByEntity(Moq.It.Is<IEnumerable<TestDoc>>(docs => docs.Contains(_expectedDoc)), _testEntity));

            static TestDoc _expectedDoc;
            static TestEntity _testEntity;

            static IEnumerable<IEntityKey> Entities { get; set; }
        }

        [Subject(typeof(DocsSelector<>))]
        class When_modify_docs_by_entities : DocsSelectorContext
        {
            Establish context = () =>
                {
                    _testEntity = new TestEntity();
                    _anotherTestEntity = new AnotherTestEntity();

                    _expectedDocs = new[] { new TestDoc(), };
                    _expectedAnotherDocs = new[] { new TestDoc(), };

                    DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(_testEntity)).Returns(_expectedDocs);
                    DocCatalog.Setup(s => s.FindDocsByRelatedPart<TestDoc>(_anotherTestEntity)).Returns(_expectedAnotherDocs);

                    Entities = new IEntityKey[] { _testEntity, _anotherTestEntity };
                };

            Because of = () => Target.ModifyDocuments(Entities).ToArray();

            It should_map_each_entity_to_docs = () =>
                {
                    DocMapper.Verify(m => m.UpdateDocByEntity(_expectedDocs, _testEntity));
                    DocMapper.Verify(m => m.UpdateDocByEntity(_expectedAnotherDocs, _anotherTestEntity));
                };

            static IEnumerable<TestDoc> _expectedDocs;
            private static IEnumerable<TestDoc> _expectedAnotherDocs;
            private static TestEntity _testEntity;
            private static AnotherTestEntity _anotherTestEntity;

            static IEnumerable<IEntityKey> Entities { get; set; }
        }

        [Subject(typeof(DocsSelector<>))]
        class When_constructed : DocsSelectorContext
        {
            It should_init_supported_doc_type_by_generic_parameter_type = () => Target.SupportedDocType.Should().Be(typeof(TestDoc));
        }

        class DocsSelectorContext
        {
            Establish context = () =>
                {
                    DocCatalog = new Mock<IRelationalDocsFinder>();
                    DocMapper = new Mock<IDocMapper<TestDoc>>();
                    QdsComponent = new Mock<IQdsComponent>();

                    Target = new DocsSelector<TestDoc>(QdsComponent.Object, DocCatalog.Object, DocMapper.Object);
                };

            protected static Mock<IDocMapper<TestDoc>> DocMapper { get; private set; }
            protected static Mock<IQdsComponent> QdsComponent { get; private set; }
            protected static Mock<IRelationalDocsFinder> DocCatalog { get; private set; }

            protected static DocsSelector<TestDoc> Target { get; private set; }
        }
    }
}