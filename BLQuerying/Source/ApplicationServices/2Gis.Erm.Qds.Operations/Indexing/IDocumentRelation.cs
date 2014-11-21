using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentRelation
    {
        Func<ElasticApi.ErmMultiGetDescriptor, ElasticApi.ErmMultiGetDescriptor> GetDocumentPartIds(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers);
        void UpdateDocumentParts(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    // ��������� ����� ������ ��� ����������� � DI
    public interface IDocumentRelation<in TDocument, TDocumentPart> : IDocumentRelation, IDocumentPartRelation
    {
    }

    public interface IDocumentPartRelation
    {
        IEnumerable<IIndexedDocumentWrapper> SelectDocumentsForPart(IReadOnlyCollection<IIndexedDocumentWrapper> documentParts, IProgress<long> progress = null);
    }
}