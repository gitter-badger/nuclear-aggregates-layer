using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentRelation
    {
        Func<ElasticApi.ErmMultiGetDescriptor, ElasticApi.ErmMultiGetDescriptor> GetDocumentPartIds(IReadOnlyCollection<IDocumentWrapper> documentWrappers);
        void UpdateDocumentParts(IReadOnlyCollection<IDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IDocumentRelation<in TDocument, TDocumentPart> : IDocumentRelation, IDocumentPartRelation
    {
    }

    public interface IDocumentPartRelation
    {
        IEnumerable<IDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IDocumentWrapper> documentParts, IProgress<long> progress = null);
    }
}