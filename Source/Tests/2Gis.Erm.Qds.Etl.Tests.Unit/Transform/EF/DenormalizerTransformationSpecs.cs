using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Extract;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class DenormalizerTransformationSpecs
    {
        [Subject(typeof(DenormalizerTransformation))]
        public class When_transform_typed_entity_set : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                var entityKeys = new[] { Mock.Of<IEntityKey>() };

                _expectedDocOne = Mock.Of<IDoc>();
                var modifierOne = CreateModifierFor(entityKeys, _expectedDocOne);

                _expectedDocTwo = Mock.Of<IDoc>();
                var modifierTwo = CreateModifierFor(entityKeys, _expectedDocTwo);

                DocumentsMetaDataSetupGetModifiers(modifierOne.Object, modifierTwo.Object);
                FillExtractedData(NewTypedEntitySet<IEntityKey>(entityKeys));
            };

            Because of = () => TargetTransform(ExtractedData);

            It should_return_collected_updates = () =>
                TransfomedDataContains(_expectedDocOne, _expectedDocTwo);

            static IDoc _expectedDocOne;
            static IDoc _expectedDocTwo;

            static Mock<IDocsSelector> CreateModifierFor(IEnumerable<IEntityKey> entities, params IDoc[] documents)
            {
                var modifier = new Mock<IDocsSelector>();
                modifier.Setup(m => m.ModifyDocuments(entities)).Returns(documents);

                return modifier;
            }

            static void TransfomedDataContains(params object[] expectedDocs)
            {
                Result.Documents.Should().Contain(expectedDocs);
            }
        }

        [Subject(typeof(DenormalizerTransformation))]
        public class When_transform_entity_set : DenormalizerTransformationContext
        {
            Establish context = () =>
                {
                    _modifier = new Mock<IDocsSelector>();
                    DocumentsMetaDataSetupGetModifiers(_modifier.Object, _modifier.Object);
                    FillExtractedData(NewTypedEntitySet<IEntityKey>(Mock.Of<IEntityKey>()));
                };

            Because of = () => TargetTransform(ExtractedData);

            It should_update_for_each_modifier_associated_with_entity_type = () =>
                _modifier.Verify(m => m.ModifyDocuments(ExtractedData.Data.First().Entities), Times.Exactly(2));

            private static Mock<IDocsSelector> _modifier;
        }

        [Subject(typeof(DenormalizerTransformation))]
        public class When_Transform_called_for_not_ErmExtractedData : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                _unsupportedData = Mock.Of<IData>();
            };

            Because of = () => _actualException = Catch.Exception(() =>
                TargetTransform(_unsupportedData));

            It should_throw_NotSupportedException = () => _actualException.Should()
                .NotBeNull("Ожидалось исключение.")
                .And
                .BeOfType<ArgumentException>("Не верный тип исключения.");

            private static IData _unsupportedData;
            private static Exception _actualException;
        }

        public class DenormalizerTransformationContext
        {
            Establish context = () =>
                {
                    Consumer = new Mock<ITransformedDataConsumer>();
                    Consumer.Setup(c => c.DataTransformed(Moq.It.IsAny<ITransformedData>()))
                        .Callback((ITransformedData td) => Result = (DenormalizedTransformedData)td);

                    DocumentsMetaData = new Mock<IDocsMetaData>();

                    Target = new DenormalizerTransformation(new ErmEntitiesDenormalizer(DocumentsMetaData.Object));
                };

            protected static Mock<IDocsMetaData> DocumentsMetaData { get; private set; }

            protected static TypedEntitySet NewTypedEntitySet<T>(params IEntityKey[] entityKeys)
            {
                return new TypedEntitySet(typeof(T), entityKeys);
            }

            protected static void TargetTransform(IData data)
            {
                Target.Transform(data, Consumer.Object);
            }

            protected static void FillExtractedData(params TypedEntitySet[] typedEntitySets)
            {
                ExtractedData = new ErmData(typedEntitySets, Mock.Of<ITrackState>());
            }

            protected static void DocumentsMetaDataSetupGetModifiers(params IDocsSelector[] modifiers)
            {
                DocumentsMetaData.Setup(d => d.GetDocsSelectors(typeof(IEntityKey))).Returns(modifiers);
            }

            protected static DenormalizerTransformation Target { get; private set; }
            protected static Mock<ITransformedDataConsumer> Consumer { get; private set; }
            protected static ErmData ExtractedData { get; private set; }
            protected static DenormalizedTransformedData Result { get; private set; }
        }
    }
}
