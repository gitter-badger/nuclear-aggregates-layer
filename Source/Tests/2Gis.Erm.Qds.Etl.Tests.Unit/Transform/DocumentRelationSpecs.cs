using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using Nest;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform
{
    class When_document_found_no_parts : UpdateDocumentPartsContext<TestDoc, TestDocPart>
    {
        const string PropertyOne = "1";

        Establish context = () =>
        {
            Documents = new[]
            {
                DocumentWrapper(null, new TestDoc { PropertyOneDoc = PropertyOne })
            };

            DocumentParts = Enumerable.Empty<IHit<TestDocPart>>();
        };

        It should_not_update_document = () =>
        {
            foreach (var document in Documents)
            {
                document.Document.PropertyOneDoc.Should().Be(PropertyOne);
            }
        };
    }

    class When_document_found_parts : UpdateDocumentPartsContext<TestDoc, TestDocPart>
    {
        const string PropertyOne = "1";

        Establish context = () =>
        {
            Documents = new[]
            {
                DocumentWrapper(null, new TestDoc { PropertyOneDoc = PropertyOne })
            };
            DocumentParts = new[]
            {
                HitMock("partId", new TestDocPart { PropertyOneDoc = "2" }),
            };

            MetadataMock.Setup(x => x.GetDocumentPartId(Moq.It.IsAny<TestDoc>())).Returns("partId");
            MetadataMock.Setup(x => x.InsertDocumentPart(Moq.It.IsAny<TestDoc>(), Moq.It.IsAny<TestDocPart>()))
                .Callback((TestDoc document, TestDocPart documentPart) => document.PropertyOneDoc = documentPart.PropertyOneDoc);
        };

        It should_update_document = () =>
        {
            foreach (var document in Documents)
            {
                document.Document.PropertyOneDoc.Should().NotBe(PropertyOne);
            }
        };
    }

    class UpdateDocumentPartsContext<TDocument, TDocumentPart> : DocumentRelationContext<TDocument, TDocumentPart>
        where TDocument : class, new()
        where TDocumentPart : class
    {
        Establish context = () =>
        {
            ElasticApiMock.Setup(x => x.Scroll(Moq.It.IsAny<Func<SearchDescriptor<TDocumentPart>, SearchDescriptor<TDocumentPart>>>())).Returns(() => DocumentParts);
        };

        Because of = () => Result = TargetFunc().UpdateDocumentParts(Documents);

        protected static ICollection<IDocumentWrapper<TDocument>> Documents;
        protected static IEnumerable<IHit<TDocumentPart>> DocumentParts;
        protected static IEnumerable<IDocumentWrapper<TDocument>> Result;
    }

    class When_documentpart_found_no_documents : SelectDocumentsToIndexForPartContext<TestDoc, TestDocPart>
    {
        private Establish context = () =>
        {
            DocumentParts = new[]
            {
                DocumentWrapper("", new TestDocPart()),
            };
            Documents = new IHit<TestDoc>[0];
        };

        It should_return_no_documents = () => Result.Should().BeEmpty();
    }

    class When_documentpart_found_documents : SelectDocumentsToIndexForPartContext<TestDoc, TestDocPart>
    {
        private Establish context = () =>
        {
            DocumentParts = new[]
            {
                DocumentWrapper("", new TestDocPart()),
            };
            Documents = new[]
            {
                HitMock(null, new TestDoc()),
            };
        };

        It should_return_documents = () => Result.Select(x => (TestDoc)x.Doc).Should().Equal(Documents.Select(x => x.Source));
    }

    class When_documentpart_update_documents : SelectDocumentsToIndexForPartContext<TestDoc, TestDocPart>
    {
        const string PropertyOne = "1";

        private Establish context = () =>
        {
            DocumentParts = new[]
            {
                DocumentWrapper("", new TestDocPart { PropertyOneDoc = PropertyOne }),
            };
            Documents = new[]
            {
                HitMock(null, new TestDoc()),
                HitMock(null, new TestDoc())
            };

            MetadataMock.Setup(x => x.InsertDocumentPart(Moq.It.IsAny<TestDoc>(), Moq.It.IsAny<TestDocPart>()))
                .Callback((TestDoc document, TestDocPart documentPart) => document.PropertyOneDoc = documentPart.PropertyOneDoc);
        };

        private It should_return_updated_documents = () =>
        {
            foreach (var result in Result)
            {
                ((TestDoc)result.Doc).ShouldHave().Properties(x => x.PropertyOneDoc).EqualTo(new TestDoc { PropertyOneDoc = PropertyOne });
            }
        };
    }

    class SelectDocumentsToIndexForPartContext<TDocument, TDocumentPart> : DocumentRelationContext<TDocument, TDocumentPart>
        where TDocument : class, new()
        where TDocumentPart : class
    {
        Establish context = () =>
        {
            ElasticApiMock.Setup(x => x.CreateBatches(Moq.It.IsAny<ICollection<IHit<TDocument>>>())).Returns(() => new[] { Documents });
            MetadataMock.Setup(x => x.GetDocumentPartId(Moq.It.IsAny<TDocument>())).Returns("");
        };

        Because of = () => Result = TargetFunc().SelectDocumentsToIndexForPart(DocumentParts);

        protected static ICollection<IDocumentWrapper<TDocumentPart>> DocumentParts;
        protected static ICollection<IHit<TDocument>> Documents;
        protected static IEnumerable<IDocumentWrapper> Result;
    }

    [Subject(typeof(DocumentRelation<,>))]
    class DocumentRelationContext<TDocument, TDocumentPart>
        where TDocument : class, new()
        where TDocumentPart : class
    {
        Establish context = () =>
        {
            ElasticApiMock = new Mock<IElasticApi>();

            var metadataContainer = new DocumentRelationMetadataContainer();
            MetadataMock = new Mock<IDocumentRelationMetadata<TDocument, TDocumentPart>>();
            metadataContainer.RegisterMetadata(() => MetadataMock.Object);

            TargetFunc = () => new DocumentRelation<TDocument, TDocumentPart>(ElasticApiMock.Object, metadataContainer);
        };

        protected static Mock<IElasticApi> ElasticApiMock;
        protected static Mock<IDocumentRelationMetadata<TDocument, TDocumentPart>> MetadataMock;
        protected static Func<DocumentRelation<TDocument, TDocumentPart>> TargetFunc;

        protected static IHit<T> HitMock<T>(string id, T document)
            where T: class
        {
            var hitMock = new Mock<IHit<T>>();
            hitMock.SetupGet(x => x.Id).Returns(id);
            hitMock.Setup(x => x.Source).Returns(document);
            return hitMock.Object;
        }

        protected static IDocumentWrapper<T> DocumentWrapper<T>(string id, T document)
            where T: class
        {
            return new DocumentWrapper<T> { Id = id, Document = document };
        }
    }
}
