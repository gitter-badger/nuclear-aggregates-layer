using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;
using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform
{
    class When_no_documents_selected : SelectDocumentsContext<TestEntity, TestDoc>
    {
        Establish context = () =>
        {
            Documents = new IDocumentWrapper<TestDoc>[0];
        };

        It should_not_return_documents = () => Result.Should().BeEmpty();
    }

    class When_documents_selected : SelectDocumentsContext<TestEntity, TestDoc>
    {
        Establish context = () =>
        {
            Documents = new[] { new TestDoc() }.Select(DocumentWrapper).ToArray();
        };

        It should_return_documents = () => Result.Should().Equal(Documents);
    }

    class SelectDocumentsContext<TEntity, TDocument> : EntityToDocumentRelationContext<TEntity, TDocument>
        where TEntity : class, IEntity
        where TDocument : class
    {
        Establish context = () =>
        {
            ElasticApiMock.Setup(x => x.CreateBatches(Moq.It.IsAny<IEnumerable<IDocumentWrapper<TDocument>>>())).Returns(() => new[] { Documents });
            DocumentRelationMock.Setup(x => x.UpdateDocumentParts(Moq.It.IsAny<ICollection<IDocumentWrapper<TDocument>>>())).Returns(() => Documents);
        };

        Because of = () => Result = TargetFunc().SelectDocuments(Enumerable.Empty<TEntity>().AsQueryable());

        protected static ICollection<IDocumentWrapper<TDocument>> Documents;
        protected static IEnumerable<IDocumentWrapper<TDocument>> Result;

        protected static IDocumentWrapper<T> DocumentWrapper<T>(T document)
            where T : class
        {
            return new DocumentWrapper<T> { Document = document };
        }
    }

    [Subject(typeof(DefaultEntityToDocumentRelation<,>))]
    class EntityToDocumentRelationContext<TEntity, TDocument>
        where TEntity : class, IEntity
    {
        Establish context = () =>
        {
            var finderMock = new Mock<IFinder>();
            ElasticApiMock = new Mock<IElasticApi>();

            DocumentRelationMock = new Mock<IDocumentRelation<TDocument>>();

            var documentRelationFactoryMock = new Mock<IDocumentRelationFactory>();
            documentRelationFactoryMock.Setup(x => x.GetDocumentRelations<TDocument>()).Returns(() => new [] {DocumentRelationMock.Object});

            TargetFunc = () => new DefaultEntityToDocumentRelation<TEntity, TDocument>(finderMock.Object, ElasticApiMock.Object, documentRelationFactoryMock.Object){SelectDocumentsFunc = x => null};
        };

        protected static Mock<IElasticApi> ElasticApiMock;
        protected static Mock<IDocumentRelation<TDocument>> DocumentRelationMock;
        protected static Func<DefaultEntityToDocumentRelation<TEntity, TDocument>> TargetFunc;
    }
}
