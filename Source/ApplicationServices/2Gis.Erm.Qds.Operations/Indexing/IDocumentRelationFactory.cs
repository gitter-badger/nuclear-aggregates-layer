using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentRelationFactory
    {
        IReadOnlyCollection<IDocumentVersionUpdater> GetDocumentVersionUpdaters(IEnumerable<Type> documentTypes);
        IReadOnlyCollection<IDocumentRelation> CreateDocumentRelations(IEnumerable<Type> documentTypes);
        IReadOnlyCollection<IDocumentPartRelation> CreateDocumentPartRelations(IEnumerable<Type> documentPartTypes);
    }
}