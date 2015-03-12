using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentRelation
    {
        Func<ErmMultiGetDescriptor, ErmMultiGetDescriptor> GetDocumentPartIds(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers);
        void UpdateDocumentParts(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IDocumentRelation<in TDocument, TDocumentPart> : IDocumentRelation, IDocumentPartRelation
    {
    }

    public interface IDocumentPartRelation
    {
        IEnumerable<IIndexedDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IIndexedDocumentWrapper> documentParts, IProgress<long> progress = null);
    }
}