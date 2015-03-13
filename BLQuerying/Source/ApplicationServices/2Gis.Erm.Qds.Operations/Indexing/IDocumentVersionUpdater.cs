using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentVersionUpdater
    {
        Func<ErmMultiGetDescriptor, ErmMultiGetDescriptor> GetDocumentVersions(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers);
        void UpdateDocumentVersions(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits);
    }

    // интерфейс нужен только для регистрации в DI
    public interface IDocumentVersionUpdater<TDocument> : IDocumentVersionUpdater
    {
    }
}