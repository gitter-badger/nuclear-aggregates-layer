using System;

using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class ErmDocsMetaDataSpecs
    {
        [Subject(typeof(ErmDocsMetaData))]
        public class When_get_modifiers_by_entity_type : ErmDocsMetaDataContext
        {
            Establish context = () =>
                {
                    _entityType = typeof(TestEntity);

                    var docType = typeof(TestDoc);
                    var docTypeTwo = typeof(AnotherTestDoc);

                    DocsMapping.Setup(dm => dm.GetRelatedDocTypes(_entityType)).Returns(new[] { docType, docTypeTwo });

                    _expectedModifier = Mock.Of<IDocsSelector>();
                    _expectedModifierTwo = Mock.Of<IDocsSelector>();

                    ModifiersRegistry.Setup(r => r.GetModifier(docType)).Returns(_expectedModifier);
                    ModifiersRegistry.Setup(r => r.GetModifier(docTypeTwo)).Returns(_expectedModifierTwo);
                };

            Because of = () => Result = Target.GetDocsSelectors(_entityType);

            It should_return_doc_modifiers_for_mapped_document_types = () =>
                    Result.Should().Contain(new[] { _expectedModifierTwo, _expectedModifier });

            static Type _entityType;
            static IDocsSelector _expectedModifier;
            static IDocsSelector _expectedModifierTwo;
            static IDocsSelector[] Result { get; set; }
        }

        public class ErmDocsMetaDataContext
        {
            Establish context = () =>
                {
                    ModifiersRegistry = new Mock<IDocModifiersRegistry>();
                    DocsMapping = new Mock<ITransformRelations>();

                    Target = new ErmDocsMetaData(ModifiersRegistry.Object, DocsMapping.Object);
                };

            protected static Mock<IDocModifiersRegistry> ModifiersRegistry;
            protected static Mock<ITransformRelations> DocsMapping;

            protected static ErmDocsMetaData Target { get; private set; }
        }
    }
}