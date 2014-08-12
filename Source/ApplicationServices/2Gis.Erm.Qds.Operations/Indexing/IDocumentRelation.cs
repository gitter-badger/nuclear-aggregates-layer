using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentVersionUpdater
    {
        Func<ElasticApi.MultiGetDescriptor2, ElasticApi.MultiGetDescriptor2> GetDocumentVersions(IReadOnlyCollection<IDocumentWrapper> documentWrappers);
        void UpdateDocumentVersions(IReadOnlyCollection<IDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IDocumentVersionUpdater<TDocument> : IDocumentVersionUpdater { }

    public interface IDocumentRelation
    {
        Func<ElasticApi.MultiGetDescriptor2, ElasticApi.MultiGetDescriptor2> GetDocumentPartIds(IReadOnlyCollection<IDocumentWrapper> documentWrappers);
        void UpdateDocumentParts(IReadOnlyCollection<IDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    public interface IDocumentPartRelation
    {
        IEnumerable<IDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IDocumentWrapper> documentParts);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IDocumentRelation<in TDocument, TDocumentPart> : IDocumentRelation, IDocumentPartRelation { }

    public interface IDocumentRelationFactory
    {
        IReadOnlyCollection<IDocumentVersionUpdater> GetDocumentVersionUpdaters(IEnumerable<Type> documentTypes);
        IReadOnlyCollection<IDocumentRelation> GetDocumentRelations(IEnumerable<Type> documentTypes);
        IReadOnlyCollection<IDocumentPartRelation> GetDocumentPartRelations(IEnumerable<Type> documentTypes);
    }
}