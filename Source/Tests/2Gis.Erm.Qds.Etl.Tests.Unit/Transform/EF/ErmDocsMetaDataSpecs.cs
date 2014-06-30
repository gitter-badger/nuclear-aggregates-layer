using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    class ErmDocsMetaDataSpecs
    {
        [Subject(typeof(ErmDocsMetaData))]
        class When_get_updaters_by_entity_type : ErmDocsMetaDataContext
        {
            Establish context = () =>
                {
                    EntityType = typeof(TestEntity);

                    var docType = typeof(TestDoc);
                    var docTypeTwo = typeof(AnotherTestDoc);

                    var result = (IEnumerable<Type>)new[] { docType, docTypeTwo };
                    DocsMapping.Setup(dm => dm.TryGetRelatedDocTypes(EntityType, out result)).Returns(true);

                    ExpectedUpdater = Mock.Of<IDocsUpdater>();
                    ExpectedUpdaterTwo = Mock.Of<IDocsUpdater>();

                    UpdatersRegistry.Setup(r => r.GetUpdater(docType)).Returns(ExpectedUpdater);
                    UpdatersRegistry.Setup(r => r.GetUpdater(docTypeTwo)).Returns(ExpectedUpdaterTwo);
                };

            Because of = () => Result = Target.GetDocsUpdaters(EntityType);

            It should_return_doc_modifiers_for_mapped_document_types = () =>
                    Result.Should().Contain(new[] { ExpectedUpdaterTwo, ExpectedUpdater });

            static Type EntityType;
            static IDocsUpdater ExpectedUpdater;
            static IDocsUpdater ExpectedUpdaterTwo;
            static IEnumerable<IDocsUpdater> Result { get; set; }
        }

        class ErmDocsMetaDataContext
        {
            Establish context = () =>
                {
                    UpdatersRegistry = new Mock<IDocUpdatersRegistry>();
                    DocsMapping = new Mock<ITransformRelations>();

                    Target = new ErmDocsMetaData(UpdatersRegistry.Object, DocsMapping.Object);
                };

            protected static Mock<IDocUpdatersRegistry> UpdatersRegistry;
            protected static Mock<ITransformRelations> DocsMapping;

            protected static ErmDocsMetaData Target { get; private set; }
        }
    }
}