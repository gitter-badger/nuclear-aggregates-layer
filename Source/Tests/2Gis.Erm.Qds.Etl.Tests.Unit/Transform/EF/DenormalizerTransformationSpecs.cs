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
        class When_transform : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                var entityKeys = new [] {Mock.Of<IEntityKey>()}.AsQueryable();

                Updater = CreateUpdaterFor(entityKeys, new[] { Mock.Of<IDoc>() });
                DocumentsMetaDataSetupGetModifiers(Updater.Object);
                FillExtractedData(NewTypedEntitySet<IEntityKey>(entityKeys));

                TargetTransform(ExtractedData);

                FillExtractedData(new TypedEntitySet[0]);
            };

            Because of = () => TargetTransform(ExtractedData);

            It should_clear_previous_transformation_results = () => Result.Documents.Should().BeEmpty();

            private static Mock<IDocsUpdater> Updater;
        }

        [Subject(typeof(DenormalizerTransformation))]
        class When_init : DenormalizerTransformationContext
        {
            Establish context = () =>
                {
                    ExpectedQdsComponents = new IQdsComponent[0];
                    QdsComponentsFactory.Setup(q => q.CreateQdsComponents()).Returns(ExpectedQdsComponents);
                };

            Because of = () => Target.Init();

            It should_bind_metadata = () => MetadataBinder.Verify(m => m.BindMetadata(ExpectedQdsComponents));

            static IEnumerable<IQdsComponent> ExpectedQdsComponents;
        }

        [Subject(typeof(DenormalizerTransformation))]
        public class When_transform_typed_entity_set : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                var entityKeys = new[] { Mock.Of<IEntityKey>() }.AsQueryable();

                ExpectedDocOne = Mock.Of<IDoc>();
                var modifierOne = CreateUpdaterFor(entityKeys, ExpectedDocOne);

                ExpectedDocTwo = Mock.Of<IDoc>();
                var modifierTwo = CreateUpdaterFor(entityKeys, ExpectedDocTwo);

                DocumentsMetaDataSetupGetModifiers(modifierOne.Object, modifierTwo.Object);
                FillExtractedData(NewTypedEntitySet<IEntityKey>(entityKeys));
            };

            Because of = () => TargetTransform(ExtractedData);

            It should_return_collected_updates = () =>
                TransfomedDataContains(ExpectedDocOne, ExpectedDocTwo);

            static IDoc ExpectedDocOne;
            static IDoc ExpectedDocTwo;

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
                    var entityKeys = new[] { Mock.Of<IEntityKey>() }.AsQueryable();
                    Updater = new Mock<IDocsUpdater>();
                    DocumentsMetaDataSetupGetModifiers(Updater.Object, Updater.Object);
                    FillExtractedData(NewTypedEntitySet<IEntityKey>(entityKeys));
                };

            Because of = () => TargetTransform(ExtractedData);

            It should_update_for_each_modifier_associated_with_entity_type = () =>
                Updater.Verify(m => m.UpdateDocuments(ExtractedData.Data.First().Entities), Times.Exactly(2));

            private static Mock<IDocsUpdater> Updater;
        }

        [Subject(typeof(DenormalizerTransformation))]
        public class When_transform_called_for_not_erm_extracted_data : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                UnsupportedData = Mock.Of<IData>();
            };

            Because of = () => ActualException = Catch.Exception(() =>
                TargetTransform(UnsupportedData));

            It should_throw_not_supported_exception = () => ActualException.Should()
                .NotBeNull("Ожидалось исключение.")
                .And
                .BeOfType<ArgumentException>("Не верный тип исключения.");

            private static IData UnsupportedData;
            private static Exception ActualException;
        }

        public class DenormalizerTransformationContext
        {
            Establish context = () =>
                {
                    Consumer = new Mock<ITransformedDataConsumer>();
                    Consumer.Setup(c => c.DataTransformed(Moq.It.IsAny<ITransformedData>()))
                        .Callback((ITransformedData td) => Result = (DenormalizedTransformedData)td);

                    DocumentsMetaData = new Mock<IDocsMetaData>();
                    MetadataBinder = new Mock<IMetadataBinder>();
                    QdsComponentsFactory = new Mock<IQdsComponentsFactory>();

                    Target = new DenormalizerTransformation(new ErmEntitiesDenormalizer(DocumentsMetaData.Object), MetadataBinder.Object, QdsComponentsFactory.Object);
                };

            // TODO Много всего, поглядеть да подумать надо
            protected static Mock<IQdsComponentsFactory> QdsComponentsFactory { get; private set; }
            protected static Mock<IMetadataBinder> MetadataBinder { get; private set; }
            protected static Mock<IDocsMetaData> DocumentsMetaData { get; private set; }
            protected static DenormalizerTransformation Target { get; private set; }
            protected static Mock<ITransformedDataConsumer> Consumer { get; private set; }
            protected static ErmData ExtractedData { get; private set; }
            protected static DenormalizedTransformedData Result { get; private set; }

            protected static Mock<IDocsUpdater> CreateUpdaterFor(IQueryable<IEntityKey> entities, params IDoc[] documents)
            {
                var modifier = new Mock<IDocsUpdater>();
                modifier.Setup(m => m.UpdateDocuments(entities)).Returns(documents);

                return modifier;
            }

            protected static TypedEntitySet NewTypedEntitySet<T>(IQueryable<IEntityKey> entityKeys)
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

            protected static void DocumentsMetaDataSetupGetModifiers(params IDocsUpdater[] modifiers)
            {
                DocumentsMetaData.Setup(d => d.GetDocsUpdaters(typeof(IEntityKey))).Returns(modifiers);
            }
        }
    }
}
