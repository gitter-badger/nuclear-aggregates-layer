using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Transform;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF
{
    public class DenormalizerTransformationSpecs
    {
        class When_transform_no_data : TransformContext
        {
            Establish context = () =>
            {
                Entities = Enumerable.Empty<IEntityKey>().AsQueryable();
                Documents = Enumerable.Empty<IDoc>();
            };

            It should_return_no_documents = () => Result.Should().BeEmpty();
        }

        class When_transform_data : TransformContext
        {
            Establish context = () =>
            {
                Entities = new[] { (IEntityKey)new TestEntity() }.AsQueryable();
                Documents = new[] { new TestDoc() };
            };

            It should_return_documents = () => Result.Should().Equal(Documents);
        }

        public class TransformContext : DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                Consumer = new Mock<ITransformedDataConsumer>();
                Consumer.Setup(c => c.DataTransformed(Moq.It.IsAny<ITransformedData>()))
                    .Callback((ITransformedData transformedData) => Result = ((DenormalizedTransformedData)transformedData).Documents);

                ExtractedData = () =>
                {
                    var trackStateMock = new Mock<ITrackState>();
                    var typedEntitySet = new TypedEntitySet(typeof(string), Entities);
                    return new ErmData(new[] { typedEntitySet }, trackStateMock.Object);
                };

                DocumentUpdaterMock.Setup(x => x.SelectDocuments(Moq.It.IsAny<IQueryable<IEntityKey>>())).Returns(() => Documents);
            };

            Because of = () => Target().Transform(ExtractedData(), Consumer.Object);

            private static Func<ErmData> ExtractedData;
            private static Mock<ITransformedDataConsumer> Consumer;

            protected static IEnumerable<IDoc> Documents;
            protected static IQueryable<IEntityKey> Entities;
            protected static IEnumerable<IDoc> Result;
        }

        [Subject(typeof(DenormalizerTransformation))]
        public class DenormalizerTransformationContext
        {
            Establish context = () =>
            {
                var documentUpdaterFactory = new Mock<IDocumentUpdaterFactory>();
                DocumentUpdaterMock = new Mock<IDocumentUpdater>();
                documentUpdaterFactory.Setup(x => x.GetDocumentUpdatersForEntityType(Moq.It.IsAny<Type>())).Returns(() => new[] { DocumentUpdaterMock.Object });

                Target = () => new DenormalizerTransformation(documentUpdaterFactory.Object);
            };

            protected static Mock<IDocumentUpdater> DocumentUpdaterMock;
            protected static Func<DenormalizerTransformation> Target;
        }
    }
}
